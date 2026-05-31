using ScadaServer.Domain.Entities;

namespace ScadaServer.Infrastructure.Communication
{
    public interface IProtocolDriver
    {
        Task<bool> ConnectAsync(string connectionStr);
        Task<object> ReadAsync(ModelVariable variable);
        
        // 新增：订阅接口
        Task SubscribeAsync(ModelVariable variable, Action<object> onValueChanged);
        Task UnsubscribeAsync(ModelVariable variable);
        
        Task DisconnectAsync();
    }
}
