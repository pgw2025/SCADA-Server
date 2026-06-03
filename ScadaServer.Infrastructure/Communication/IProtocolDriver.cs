using ScadaServer.Domain.Entities;

namespace ScadaServer.Infrastructure.Communication
{
    public interface IProtocolDriver
    {
        Task<bool> ConnectAsync(Device device);
        Task<object> ReadAsync(ModelVariable variable);
        Task<IDictionary<string, object>> ReadBatchAsync(IEnumerable<ModelVariable> variables);
        
        // 新增：批量订阅接口
        Task SubscribeAsync(IEnumerable<ModelVariable> variables, Action<string, object> onValueChanged);
        Task UnsubscribeAsync(IEnumerable<ModelVariable> variables);
        
        Task DisconnectAsync();
    }
}
