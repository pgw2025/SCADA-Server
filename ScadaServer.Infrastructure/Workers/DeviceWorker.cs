using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ScadaServer.Application.Interfaces;
using ScadaServer.Domain.Entities;
using ScadaServer.Domain.Enums;
using ScadaServer.Infrastructure.Communication;
using ScadaServer.Infrastructure.Services;
using System.Collections.Concurrent;

namespace ScadaServer.Infrastructure.Workers
{
    public class DeviceWorker : BackgroundService, IDeviceRuntimeManager
    {
        private readonly ILogger<DeviceWorker> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly DeviceRegistry _registry;
        private readonly IScadaNotificationService _notificationService;
        private readonly IProtocolDriverFactory _driverFactory;
        
        // 改进：同时存储 Cts 和 Task 句柄
        private readonly ConcurrentDictionary<int, (CancellationTokenSource Cts, Task WorkerTask)> _deviceTasks = new();
        private readonly SemaphoreSlim _lock = new(1, 1);
        private CancellationToken _stoppingToken;

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

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _stoppingToken = stoppingToken;
            _logger.LogInformation("Acquisition Engine Started.");
            
            // 初始加载
            await ReloadAll();

            // 保持服务运行
            try
            {
                await Task.Delay(Timeout.Infinite, stoppingToken);
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("Acquisition Engine Stopping...");
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

                var device = await deviceRepo.GetByIdAsync(deviceId);
                if (device == null)
                {
                    _registry.RemoveDevice(deviceId);
                    return;
                }

                var variables = (await varRepo.GetListAsync(v => v.ModelId == device.ModelId)).ToList();
                _registry.UpdateDevice(device, variables);

                StartDeviceTask(device, variables);
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

                var devices = await deviceRepo.GetListAsync();
                var activeDeviceIds = devices.Select(d => d.Id).ToHashSet();

                // 1. 清理已删除的设备任务
                foreach (var deviceId in _deviceTasks.Keys)
                {
                    if (!activeDeviceIds.Contains(deviceId))
                    {
                        await StopDeviceInternal(deviceId);
                        _registry.RemoveDevice(deviceId);
                    }
                }

                // 2. 启动或更新当前设备任务
                foreach (var device in devices)
                {
                    var variables = (await varRepo.GetListAsync(v => v.ModelId == device.ModelId)).ToList();
                    _registry.UpdateDevice(device, variables);
                    
                    if (!_deviceTasks.ContainsKey(device.Id))
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
            // 关联全局 Token，确保服务停止时所有任务也停止
            var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(_stoppingToken, cts.Token);
            
            // 使用 LongRunning 选项，优化长生命周期任务的线程调度
            var task = Task.Factory.StartNew(
                () => RunDeviceOrchestratorWithRetry(device, variables, linkedCts.Token),
                linkedCts.Token,
                TaskCreationOptions.LongRunning,
                TaskScheduler.Default
            ).Unwrap();

            // 监控任务异常崩溃
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
                    // 等待该任务优雅退出
                    await Task.WhenAny(item.WorkerTask, Task.Delay(2000));
                }
                catch (ObjectDisposedException) { }
                finally
                {
                    item.Cts.Dispose();
                }
            }
        }

        private async Task UpdateStatusAsync(int deviceId, DeviceStatus status)
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var deviceRepo = scope.ServiceProvider.GetRequiredService<IDeviceRepository>();
                var device = await deviceRepo.GetByIdAsync(deviceId);
                if (device != null)
                {
                    device.Status = status;
                    device.LastUpdated = DateTime.Now;
                    await deviceRepo.UpdateAsync(device);
                    _logger.LogInformation($"Updated device {device.Name} status to {status}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning($"Failed to update device {deviceId} status: {ex.Message}");
            }
        }

        private async Task RunDeviceOrchestratorWithRetry(Device device, List<ModelVariable> variables, CancellationToken stoppingToken)
        {
            var driver = _driverFactory.CreateDriver(device.Type);
            if (driver == null)
            {
                _logger.LogError($"Unsupported device type: {device.Type} for device {device.Name}");
                return;
            }

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    _logger.LogInformation($"Connecting to {device.Name} ({device.IpAddress})...");
                    if (await driver.ConnectAsync(device))
                    {
                        _logger.LogInformation($"Connected to {device.Name}. Starting acquisition tasks.");
                        await UpdateStatusAsync(device.Id, DeviceStatus.Online);

                        var subscriptionVars = variables.Where(v => v.UpdateMode == UpdateMode.Subscription).ToList();
                        var pollingVars = variables.Where(v => v.UpdateMode == UpdateMode.Polling).ToList();

                        var tasks = new List<Task>();

                        if (subscriptionVars.Any())
                        {
                            await driver.SubscribeAsync(subscriptionVars, (key, val) =>
                            {
                                _ = _notificationService.NotifyVariableUpdateAsync(device.Id, key, val);
                            });
                        }

                        var pollingGroups = pollingVars.GroupBy(v => v.PollingIntervalMs);
                        foreach (var group in pollingGroups)
                        {
                            tasks.Add(RunPollingLoop(driver, device, group.ToList(), group.Key, stoppingToken));
                        }

                        if (tasks.Any())
                        {
                            await Task.WhenAll(tasks);
                        }
                        else if (subscriptionVars.Any())
                        {
                            // 只有订阅变量，则保持连接并等待停止信号
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
                        await UpdateStatusAsync(device.Id, DeviceStatus.Offline);
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
                    await UpdateStatusAsync(device.Id, DeviceStatus.Fault);
                    try
                    {
                        await Task.Delay(5000, stoppingToken);
                    }
                    catch (OperationCanceledException) { break; }
                }
                finally
                {
                    try
                    {
                        await driver.DisconnectAsync();
                        await UpdateStatusAsync(device.Id, DeviceStatus.Offline);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogDebug(ex, $"Error during disconnect for {device.Name}");
                    }
                }
            }
            _logger.LogInformation($"Acquisition loop stopped for device {device.Name}");
        }

        private async Task RunPollingLoop(IProtocolDriver driver, Device device, List<ModelVariable> variables, int intervalMs, CancellationToken stoppingToken)
        {
            using var timer = new PeriodicTimer(TimeSpan.FromMilliseconds(intervalMs));
            while (!stoppingToken.IsCancellationRequested && await timer.WaitForNextTickAsync(stoppingToken))
            {
                try
                {
                    var results = await driver.ReadBatchAsync(variables);
                    foreach (var result in results)
                    {
                        await _notificationService.NotifyVariableUpdateAsync(device.Id, result.Key, result.Value);
                        SystemMonitorService.IncrementPollPackets();
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning($"Polling error for group {intervalMs}ms: {ex.Message}");
                }
            }
        }
    }
}
