using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ScadaServer.Application.Interfaces;
using ScadaServer.Domain.Entities;
using ScadaServer.Domain.Enums;
using ScadaServer.Infrastructure.Communication;
using ScadaServer.Infrastructure.Services;
using System.Collections.Concurrent;
using System.Threading.Channels;
using System.Diagnostics;

namespace ScadaServer.Infrastructure.Workers
{
    public class DeviceWorker : BackgroundService, IDeviceRuntimeManager
    {
        private readonly ILogger<DeviceWorker> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly DeviceRegistry _registry;
        private readonly IScadaNotificationService _notificationService;
        private readonly IProtocolDriverFactory _driverFactory;

        // 设备任务管理
        private readonly ConcurrentDictionary<int, (CancellationTokenSource Cts, Task WorkerTask)> _deviceTasks = new();
        
        // 运行时状态（内存维护，不持久化）
        private readonly ConcurrentDictionary<int, DeviceRuntime> _runtimeCache = new();
        
        // Key -> DeviceId 映射（快速查找）
        private readonly ConcurrentDictionary<string, int> _keyIndex = new();
        
        private readonly Channel<VariableUpdate> _notificationChannel = Channel.CreateUnbounded<VariableUpdate>(
            new UnboundedChannelOptions { SingleReader = true });
        private readonly SemaphoreSlim _lock = new(1, 1);
        private CancellationToken _stoppingToken;

        public record VariableUpdate(int DeviceId, string Key, object Value);

        public DeviceWorker(
            ILogger<DeviceWorker> logger,
            IServiceProvider serviceProvider,
            DeviceRegistry registry,
            IScadaNotificationService notificationService,
            IProtocolDriverFactory driverFactory)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            _registry = registry;
            _notificationService = notificationService;
            _driverFactory = driverFactory;
        }

        /// <summary>
        /// 获取设备运行时状态
        /// </summary>
        public DeviceRuntime? GetRuntime(int deviceId)
        {
            return _runtimeCache.TryGetValue(deviceId, out var runtime) ? runtime : null;
        }

        /// <summary>
        /// 根据 Key 获取设备运行时状态
        /// </summary>
        public DeviceRuntime? GetRuntimeByKey(string key)
        {
            if (_keyIndex.TryGetValue(key, out var deviceId))
            {
                return GetRuntime(deviceId);
            }
            return null;
        }

        /// <summary>
        /// 获取所有设备运行时状态
        /// </summary>
        public IEnumerable<DeviceRuntime> GetAllRuntimes() => _runtimeCache.Values;

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _stoppingToken = stoppingToken;
            _logger.LogInformation("Acquisition Engine Started.");

            // 启动通知处理器
            var notificationTask = ProcessNotificationsAsync(stoppingToken);

            // 初始加载
            await ReloadAll();

            // 保持服务运行
            try
            {
                await Task.WhenAll(notificationTask, Task.Delay(Timeout.Infinite, stoppingToken));
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("Acquisition Engine Stopping...");
            }
        }

        private async Task ProcessNotificationsAsync(CancellationToken stoppingToken)
        {
            await foreach (var update in _notificationChannel.Reader.ReadAllAsync(stoppingToken))
            {
                try
                {
                    await _notificationService.NotifyVariableUpdateAsync(update.DeviceId, update.Key, update.Value);

                    // 更新运行时状态
                    if (_runtimeCache.TryGetValue(update.DeviceId, out var runtime))
                    {
                        runtime.LastCommunicationTime = DateTime.Now;
                        runtime.SuccessCount++;
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning($"Failed to process notification for device {update.DeviceId}: {ex.Message}");
                }
            }
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Executing graceful shutdown for all acquisition tasks...");

            // 1. 触发所有任务取消信号
            foreach (var item in _deviceTasks.Values)
            {
                item.Cts.Cancel();
            }

            // 2. 等待所有后台任务完成（最多等待5秒）
            var tasks = _deviceTasks.Values.Select(x => x.WorkerTask).ToList();
            if (tasks.Any())
            {
                await Task.WhenAny(Task.WhenAll(tasks), Task.Delay(5000, cancellationToken));
            }

            // 3. 释放资源
            foreach (var item in _deviceTasks.Values)
            {
                item.Cts.Dispose();
            }

            await base.StopAsync(cancellationToken);
        }

        public async Task RefreshDevice(int deviceId)
        {
            await _lock.WaitAsync();
            try
            {
                await StopDeviceInternal(deviceId);

                using var scope = _serviceProvider.CreateScope();
                var deviceRepo = scope.ServiceProvider.GetRequiredService<IDeviceRepository>();
                var varRepo = scope.ServiceProvider.GetRequiredService<IRepository<ModelVariable>>();

                var device = await deviceRepo.GetByIdWithConfigAsync(deviceId);
                if (device == null)
                {
                    _registry.RemoveDevice(deviceId);
                    _runtimeCache.TryRemove(deviceId, out _);
                    _keyIndex.TryRemove(device.Key, out _);
                    return;
                }

                var variables = (await varRepo.GetListAsync(v => v.ModelId == device.ModelId)).ToList();
                _registry.UpdateDevice(device, variables);

                // 更新 Key 索引
                _keyIndex[device.Key] = device.Id;

                // 只有启用的设备才启动采集
                if (device.IsEnabled)
                {
                    StartDeviceTask(device, variables);
                }
            }
            finally
            {
                _lock.Release();
            }
        }

        public async Task ReloadAll()
        {
            await _lock.WaitAsync();
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var deviceRepo = scope.ServiceProvider.GetRequiredService<IDeviceRepository>();
                var varRepo = scope.ServiceProvider.GetRequiredService<IRepository<ModelVariable>>();

                var devices = await deviceRepo.GetListWithConfigAsync();
                var activeDeviceIds = devices.Select(d => d.Id).ToHashSet();

                // 批量加载所有变量
                var allVariables = await varRepo.GetListAsync();
                var variableLookup = allVariables.ToLookup(v => v.ModelId);

                // 1. 清理已删除的设备任务
                foreach (var deviceId in _deviceTasks.Keys)
                {
                    if (!activeDeviceIds.Contains(deviceId))
                    {
                        await StopDeviceInternal(deviceId);
                        _registry.RemoveDevice(deviceId);
                        _runtimeCache.TryRemove(deviceId, out _);
                    }
                }

                // 2. 启动或更新当前设备任务
                foreach (var device in devices)
                {
                    var variables = variableLookup[device.ModelId].ToList();
                    _registry.UpdateDevice(device, variables);

                    // 更新 Key 索引
                    _keyIndex[device.Key] = device.Id;

                    // 初始化运行时状态
                    if (!_runtimeCache.ContainsKey(device.Id))
                    {
                        _runtimeCache[device.Id] = new DeviceRuntime
                        {
                            DeviceId = device.Id,
                            DeviceName = device.Name,
                            CurrentStatus = DeviceStatus.Offline
                        };
                    }

                    // 只有启用的设备才启动采集
                    if (device.IsEnabled && !_deviceTasks.ContainsKey(device.Id))
                    {
                        StartDeviceTask(device, variables);
                    }
                }
            }
            finally
            {
                _lock.Release();
            }
        }

        private void StartDeviceTask(Device device, List<ModelVariable> variables)
        {
            var cts = new CancellationTokenSource();
            var runtime = _runtimeCache.GetOrAdd(device.Id, _ => new DeviceRuntime
            {
                DeviceId = device.Id,
                DeviceName = device.Name,
                CurrentStatus = DeviceStatus.Offline
            });

            // 使用 LongRunning 选项
            var task = Task.Factory.StartNew(
                async () =>
                {
                    using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(_stoppingToken, cts.Token);
                    try
                    {
                        await RunDeviceOrchestratorWithRetry(device, variables, runtime, linkedCts.Token);
                    }
                    catch (OperationCanceledException) { }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, $"Fatal error in RunDeviceOrchestratorWithRetry for {device.Name}");
                    }
                },
                cts.Token,
                TaskCreationOptions.LongRunning,
                TaskScheduler.Default
            ).Unwrap();

            // 监控任务异常
            _ = task.ContinueWith(t =>
            {
                if (t.IsFaulted)
                {
                    _logger.LogCritical(t.Exception, $"Fatal error in acquisition task for device: {device.Name}");
                }
            }, TaskContinuationOptions.OnlyOnFaulted);

            _deviceTasks[device.Id] = (cts, task);
        }

        private async Task StopDeviceInternal(int deviceId)
        {
            if (_deviceTasks.TryRemove(deviceId, out var item))
            {
                try
                {
                    item.Cts.Cancel();
                    await Task.WhenAny(item.WorkerTask, Task.Delay(2000));
                }
                catch (ObjectDisposedException) { }
                finally
                {
                    item.Cts.Dispose();
                }

                // 更新运行时状态
                if (_runtimeCache.TryGetValue(deviceId, out var runtime))
                {
                    runtime.CurrentStatus = DeviceStatus.Offline;
                }
            }
        }

        private void UpdateRuntimeStatus(DeviceRuntime runtime, DeviceStatus status, string? error = null)
        {
            runtime.CurrentStatus = status;
            runtime.LastHeartbeat = DateTime.Now;

            if (status == DeviceStatus.Fault)
            {
                runtime.LastError = error;
                runtime.FailureCount++;
            }

            if (status == DeviceStatus.Online)
            {
                runtime.LastError = null;
            }
        }

        private async Task RunDeviceOrchestratorWithRetry(
            Device device, 
            List<ModelVariable> variables, 
            DeviceRuntime runtime,
            CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                IProtocolDriver? driver = null;
                try
                {
                    // 优先使用 DriverName，否则使用 Type
                    driver = !string.IsNullOrEmpty(device.DriverName)
                        ? _driverFactory.CreateDriver(device.DriverName)
                        : _driverFactory.CreateDriver(device.Type);

                    if (driver == null)
                    {
                        _logger.LogError($"Unsupported device type: {device.Type} for device {device.Name}");
                        UpdateRuntimeStatus(runtime, DeviceStatus.Fault, $"不支持的设备类型: {device.Type}");
                        return;
                    }

                    // 检查配置是否存在
                    if (device.Config == null || string.IsNullOrEmpty(device.Config.JsonConfig))
                    {
                        _logger.LogError($"Device {device.Name} has no protocol configuration");
                        UpdateRuntimeStatus(runtime, DeviceStatus.Fault, "缺少协议配置");
                        return;
                    }

                    _logger.LogInformation($"Connecting to {device.Name}...");
                    UpdateRuntimeStatus(runtime, DeviceStatus.Connecting);

                    if (await driver.ConnectAsync(device, device.Config.JsonConfig))
                    {
                        _logger.LogInformation($"Connected to {device.Name}. Starting acquisition tasks.");
                        UpdateRuntimeStatus(runtime, DeviceStatus.Online);

                        var subscriptionVars = variables.Where(v => v.UpdateMode == UpdateMode.Subscription).ToList();
                        var pollingVars = variables.Where(v => v.UpdateMode == UpdateMode.Polling).ToList();

                        var tasks = new List<Task>();

                        if (subscriptionVars.Any())
                        {
                            await driver.SubscribeAsync(subscriptionVars, (key, val) =>
                            {
                                _notificationChannel.Writer.TryWrite(new VariableUpdate(device.Id, key, val));
                            });
                        }

                        // 使用设备配置的采集周期作为默认值
                        var pollingGroups = pollingVars.GroupBy(v => v.PollingIntervalMs > 0 ? v.PollingIntervalMs : device.PollingInterval);
                        foreach (var group in pollingGroups)
                        {
                            tasks.Add(RunPollingLoop(driver, device, runtime, group.ToList(), group.Key, stoppingToken));
                        }

                        if (tasks.Any())
                        {
                            await Task.WhenAll(tasks);
                        }
                        else if (subscriptionVars.Any())
                        {
                            await Task.Delay(Timeout.Infinite, stoppingToken);
                        }
                        else
                        {
                            _logger.LogWarning($"Device {device.Name} has no variables configured. Waiting 10s...");
                            await Task.Delay(10000, stoppingToken);
                        }
                    }
                    else
                    {
                        _logger.LogWarning($"Failed to connect to {device.Name}. Will retry in 10s.");
                        UpdateRuntimeStatus(runtime, DeviceStatus.Offline, "连接失败");
                        runtime.ReconnectCount++;
                        await Task.Delay(10000, stoppingToken);
                    }
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Error in acquisition for {device.Name}. Retrying in 5s...");
                    UpdateRuntimeStatus(runtime, DeviceStatus.Fault, ex.Message);
                    runtime.ReconnectCount++;
                    try
                    {
                        await Task.Delay(5000, stoppingToken);
                    }
                    catch (OperationCanceledException) { break; }
                }
                finally
                {
                    if (driver != null)
                    {
                        try
                        {
                            await driver.DisconnectAsync();
                            await driver.DisposeAsync();
                        }
                        catch (Exception ex)
                        {
                            _logger.LogDebug(ex, $"Error during driver cleanup for {device.Name}");
                        }
                    }
                    UpdateRuntimeStatus(runtime, DeviceStatus.Offline);
                }
            }
            _logger.LogInformation($"Acquisition loop stopped for device {device.Name}");
        }

        private async Task RunPollingLoop(
            IProtocolDriver driver, 
            Device device, 
            DeviceRuntime runtime,
            List<ModelVariable> variables, 
            int intervalMs, 
            CancellationToken stoppingToken)
        {
            using var timer = new PeriodicTimer(TimeSpan.FromMilliseconds(intervalMs));
            var sw = new Stopwatch();

            while (!stoppingToken.IsCancellationRequested && await timer.WaitForNextTickAsync(stoppingToken))
            {
                try
                {
                    sw.Restart();
                    var results = await driver.ReadBatchAsync(variables);
                    sw.Stop();

                    foreach (var result in results)
                    {
                        _notificationChannel.Writer.TryWrite(new VariableUpdate(device.Id, result.Key, result.Value));
                        SystemMonitorService.IncrementPollPackets();
                    }

                    // 如果读取耗时超过周期，记录警告
                    if (sw.ElapsedMilliseconds > intervalMs)
                    {
                        _logger.LogWarning($"Polling drift detected for {device.Name} ({intervalMs}ms group). Actual read time: {sw.ElapsedMilliseconds}ms");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning($"Polling error for group {intervalMs}ms on {device.Name}: {ex.Message}");
                    runtime.FailureCount++;
                }
            }
        }
    }
}
