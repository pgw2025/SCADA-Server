using ScadaServer.Domain.Entities;
using Opc.Ua.Client;

namespace ScadaServer.Infrastructure.Communication
{
    public class OpcUaDriver : IProtocolDriver
    {
        public async Task<bool> ConnectAsync(string connectionStr)
        {
            // Implementation for OPC UA
            await Task.CompletedTask;
            return true;
        }

        public async Task<object> ReadAsync(MetricConfig config)
        {
            // Implementation for OPC UA
            await Task.CompletedTask;
            return 25.5; // Dummy data
        }

        public async Task DisconnectAsync()
        {
            await Task.CompletedTask;
        }
    }
}
