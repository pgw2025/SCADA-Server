using ScadaServer.Domain.Entities;

namespace ScadaServer.Infrastructure.Communication
{
    public interface IProtocolDriver
    {
        Task<bool> ConnectAsync(string connectionStr);
        Task<object> ReadAsync(MetricConfig config);
        Task DisconnectAsync();
    }
}
