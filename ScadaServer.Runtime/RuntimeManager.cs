using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using ScadaServer.Runtime.Devices;
using ScadaServer.Runtime.Interface;

namespace ScadaServer.Runtime
{
    public class RuntimeManager : IRuntimeManager
    {
        public ConcurrentDictionary<int, DeviceRuntime> DeviceRuntimes { get; } = new();

        private readonly ILogger<RuntimeManager> _logger;
        private DeviceScheduler? _scheduler;

        public RuntimeManager(ILogger<RuntimeManager> logger)
        {
            _logger = logger;
        }

        public void RegisterDevice(DeviceRuntime runtime)
        {
            DeviceRuntimes[runtime.Device.Id] = runtime;
        }

        public async Task StartAsync(CancellationToken token, int maxConcurrentWorkers)
        {
            // _scheduler = new DeviceScheduler(this, maxConcurrentWorkers, _logger);
            // await _scheduler.StartAsync(token);
        }

        public Task InitializeAsync()
        {
            return Task.CompletedTask;
        }
    }
}