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
        private readonly ConcurrentDictionary<int, CancellationTokenSource> _deviceCts = new();
        private readonly SemaphoreSlim _lock = new(1, 1);

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
            _logger.LogInformation("Acquisition Engine Started.");
            await ReloadAll();

            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(10000, stoppingToken);
            }
        }

        public async Task RefreshDevice(int deviceId)
        {
            await _lock.WaitAsync();
            try
            {
                CancelAndDisposeCts(deviceId);

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

                var newCts = new CancellationTokenSource();
                _deviceCts[deviceId] = newCts;
                
                _ = Task.Run(() => RunDeviceOrchestratorWithRetry(device, variables, newCts.Token), newCts.Token);
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

                // 1. Cleanup tasks for devices that no longer exist
                foreach (var deviceId in _deviceCts.Keys)
                {
                    if (!activeDeviceIds.Contains(deviceId))
                    {
                        CancelAndDisposeCts(deviceId);
                        _registry.RemoveDevice(deviceId);
                    }
                }

                // 2. Start or update current device tasks
                foreach (var device in devices)
                {
                    var variables = (await varRepo.GetListAsync(v => v.ModelId == device.ModelId)).ToList();
                    _registry.UpdateDevice(device, variables);
                    
                    if (!_deviceCts.ContainsKey(device.Id))
                    {
                        var cts = new CancellationTokenSource();
                        _deviceCts[device.Id] = cts;
                        _ = Task.Run(() => RunDeviceOrchestratorWithRetry(device, variables, cts.Token), cts.Token);
                    }
                }
            }
            finally
            {
                _lock.Release();
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

                        var subscriptionVars = variables.Where(v => v.UpdateMode == UpdateMode.Subscription).ToList();
                        var pollingVars = variables.Where(v => v.UpdateMode == UpdateMode.Polling).ToList();

                        var tasks = new List<Task>();

                        if (subscriptionVars.Any())
                        {
                            await driver.SubscribeAsync(subscriptionVars, (key, val) =>
                            {
                                _ = _notificationService.NotifyVariableUpdateAsync(key, val);
                            });
                        }

                        var pollingGroups = pollingVars.GroupBy(v => v.PollingIntervalMs);
                        foreach (var group in pollingGroups)
                        {
                            tasks.Add(RunPollingLoop(driver, group.ToList(), group.Key, stoppingToken));
                        }

                        await Task.WhenAll(tasks);
                    }
                    else
                    {
                        _logger.LogWarning($"Failed to connect to {device.Name}. Will retry in 10s.");
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
                    }
                    catch (Exception ex)
                    {
                        _logger.LogDebug(ex, $"Error during disconnect for {device.Name}");
                    }
                }
            }
            _logger.LogInformation($"Acquisition loop stopped for device {device.Name}");
        }

        private async Task RunPollingLoop(IProtocolDriver driver, List<ModelVariable> variables, int intervalMs, CancellationToken stoppingToken)
        {
            using var timer = new PeriodicTimer(TimeSpan.FromMilliseconds(intervalMs));
            while (!stoppingToken.IsCancellationRequested && await timer.WaitForNextTickAsync(stoppingToken))
            {
                try
                {
                    var results = await driver.ReadBatchAsync(variables);
                    foreach (var result in results)
                    {
                        await _notificationService.NotifyVariableUpdateAsync(result.Key, result.Value);
                        SystemMonitorService.IncrementPollPackets();
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning($"Polling error for group {intervalMs}ms: {ex.Message}");
                }
            }
        }

        private void CancelAndDisposeCts(int deviceId)
        {
            if (_deviceCts.TryRemove(deviceId, out var cts))
            {
                try
                {
                    cts.Cancel();
                }
                catch (ObjectDisposedException) { }
                finally
                {
                    cts.Dispose();
                }
            }
        }
    }
}
