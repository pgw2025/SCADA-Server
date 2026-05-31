using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ScadaServer.Application.Interfaces;
using ScadaServer.Domain.Entities;
using ScadaServer.Infrastructure.Communication;

namespace ScadaServer.Infrastructure.Workers
{
    public class DeviceWorker : BackgroundService
    {
        private readonly ILogger<DeviceWorker> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly Dictionary<int, Task> _runningTasks = new();

        public DeviceWorker(ILogger<DeviceWorker> logger, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Acquisition Engine Started.");

            while (!stoppingToken.IsCancellationRequested)
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var deviceRepo = scope.ServiceProvider.GetRequiredService<IDeviceRepository>();
                    var metricRepo = scope.ServiceProvider.GetRequiredService<IRepository<MetricConfig>>();
                    
                    var devices = await deviceRepo.GetListAsync();
                    foreach (var device in devices)
                    {
                        if (!_runningTasks.ContainsKey(device.Id))
                        {
                            var metrics = (await metricRepo.GetListAsync()).Where(m => m.DeviceId == device.Id && m.IsActive).ToList();
                            _runningTasks[device.Id] = Task.Run(() => RunDeviceAcquisition(device, metrics, stoppingToken), stoppingToken);
                        }
                    }
                }

                await Task.Delay(10000, stoppingToken); // Check for new devices every 10s
            }
        }

        private async Task RunDeviceAcquisition(DeviceEntity device, List<MetricConfig> metrics, CancellationToken stoppingToken)
        {
            _logger.LogInformation($"Starting acquisition for {device.Name}");
            
            IProtocolDriver driver = device.ProtocolType == "S7" ? new S7Driver() : new OpcUaDriver();
            
            try 
            {
                if (await driver.ConnectAsync(device.ConnectionStr))
                {
                    while (!stoppingToken.IsCancellationRequested)
                    {
                        foreach (var metric in metrics)
                        {
                            var val = await driver.ReadAsync(metric);
                            _logger.LogInformation($"Device: {device.Name}, Metric: {metric.KeyName}, Value: {val}");
                        }
                        await Task.Delay(1000, stoppingToken); // Example global interval
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
