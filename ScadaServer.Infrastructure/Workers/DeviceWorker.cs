using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ScadaServer.Application.Interfaces;
using ScadaServer.Domain.Entities;
using ScadaServer.Infrastructure.Communication;
using System.Collections.Concurrent;

namespace ScadaServer.Infrastructure.Workers
{
    public class DeviceWorker : BackgroundService, IDeviceRuntimeManager
    {
        private readonly ILogger<DeviceWorker> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly DeviceRegistry _registry;
        private readonly ConcurrentDictionary<int, CancellationTokenSource> _deviceTasks = new();

        public DeviceWorker(ILogger<DeviceWorker> logger, IServiceProvider serviceProvider, DeviceRegistry registry)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            _registry = registry;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Acquisition Engine Started.");
            await ReloadAll();

            // 监听配置变更或新设备发现逻辑可以在此添加
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(5000, stoppingToken);
            }
        }

        public async void RefreshDevice(int deviceId)
        {
            // 取消旧任务
            if (_deviceTasks.TryRemove(deviceId, out var cts))
            {
                cts.Cancel();
            }

            // 重新从DB加载配置到Registry
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

            // 启动新任务
            var newCts = new CancellationTokenSource();
            _deviceTasks[deviceId] = newCts;
            _ = Task.Run(() => RunDeviceAcquisition(deviceId, newCts.Token), newCts.Token);
        }

        public async void ReloadAll()
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

            var (device, metrics) = config.Value;
            _logger.LogInformation($"Starting acquisition for {device.Name}");
            
            IProtocolDriver driver = device.Type == "S7" ? new S7Driver() : new OpcUaDriver(); // 假设Type字段映射到协议类型
            
            try 
            {
                if (await driver.ConnectAsync(device.IpAddress)) // 假设连接字符串是IpAddress
                {
                    while (!stoppingToken.IsCancellationRequested)
                    {
                        foreach (var metric in metrics)
                        {
                            var val = await driver.ReadAsync(metric);
                            // 这里添加推送给 SignalR 的逻辑
                            _logger.LogInformation($"Device: {device.Name}, Metric: {metric.Key}, Value: {val}");
                        }
                        await Task.Delay(1000, stoppingToken);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error in acquisition for {device.Name}");
            }
            finally
            {
                await driver.DisconnectAsync();
            }
        }
    }
}
