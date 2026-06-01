
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ScadaServer.Application.Interfaces;
using ScadaServer.Domain.Entities;
using ScadaServer.Domain.Enums;
using ScadaServer.Infrastructure.Communication;

using System.Collections.Concurrent;

namespace ScadaServer.Infrastructure.Workers
{
    public class DeviceWorker : BackgroundService, IDeviceRuntimeManager
    {
        private readonly ILogger<DeviceWorker> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly DeviceRegistry _registry;
        private readonly IScadaNotificationService _notificationService;
        private readonly ConcurrentDictionary<int, CancellationTokenSource> _deviceTasks = new();

        public DeviceWorker(ILogger<DeviceWorker> logger, IServiceProvider serviceProvider, DeviceRegistry registry, IScadaNotificationService notificationService)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            _registry = registry;
            _notificationService = notificationService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Acquisition Engine Started.");
            await ReloadAll();

            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(5000, stoppingToken);
            }
        }

        public async Task RefreshDevice(int deviceId)
        {
            if (_deviceTasks.TryRemove(deviceId, out var cts))
            {
                cts.Cancel();
            }

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
            _deviceTasks[deviceId] = newCts;
            _ = Task.Run(() => RunDeviceAcquisition(deviceId, newCts.Token), newCts.Token);
        }

        public async Task ReloadAll()
        {
            using var scope = _serviceProvider.CreateScope();
            var deviceRepo = scope.ServiceProvider.GetRequiredService<IDeviceRepository>();
            var varRepo = scope.ServiceProvider.GetRequiredService<IRepository<ModelVariable>>();

            var devices = await deviceRepo.GetListAsync();
            foreach (var device in devices)
            {
                var variables = (await varRepo.GetListAsync(v => v.ModelId == device.ModelId)).ToList();
                _registry.UpdateDevice(device, variables);
                
                if (!_deviceTasks.ContainsKey(device.Id))
                {
                    var cts = new CancellationTokenSource();
                    _deviceTasks[device.Id] = cts;
                    _ = Task.Run(() => RunDeviceAcquisition(device.Id, cts.Token), cts.Token);
                }
            }
        }

        private async Task RunDeviceAcquisition(int deviceId, CancellationToken stoppingToken)
        {
            var config = _registry.GetDeviceConfig(deviceId);
            if (config == null) return;

            var (device, allVariables) = config.Value;
            var pollingVars = allVariables.Where(v => v.UpdateMode == UpdateMode.Polling).ToList();
            var subscriptionVars = allVariables.Where(v => v.UpdateMode == UpdateMode.Subscription).ToList();

            IProtocolDriver driver = device.Type == "S7" ? new S7Driver() : new OpcUaDriver();
            
            try 
            {
                if (await driver.ConnectAsync(device.IpAddress))
                {
                    // 订阅模式
                    foreach (var v in subscriptionVars) {
                        await driver.SubscribeAsync(v, (val) => {
                             _notificationService.NotifyVariableUpdateAsync(v.Key, val);
                        });
                    }

                    // 轮询模式
                    while (!stoppingToken.IsCancellationRequested)
                    {
                        foreach (var v in pollingVars)
                        {
                            var val = await driver.ReadAsync(v);
                            await _notificationService.NotifyVariableUpdateAsync(v.Key, val);
                        }
                        await Task.Delay(1000, stoppingToken);
                    }
                }
            }
            finally
            {
                foreach (var v in subscriptionVars) await driver.UnsubscribeAsync(v);
                await driver.DisconnectAsync();
            }
        }
    }
}

