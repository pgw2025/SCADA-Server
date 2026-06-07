using ScadaServer.Domain.Entities;

namespace ScadaServer.Domain.Interfaces
{
    public interface IProtocolDriver : IAsyncDisposable
    {
        Task<bool> ConnectAsync(Device device, string configJson);
        
        Task<object> ReadAsync(ModelVariable variable);
        Task<IDictionary<string, object>> ReadBatchAsync(IEnumerable<ModelVariable> variables);
        
        Task SubscribeAsync(IEnumerable<ModelVariable> variables, Action<string, object> onValueChanged);
        Task UnsubscribeAsync(IEnumerable<ModelVariable> variables);
        
        Task DisconnectAsync();
    }
}
