using ScadaServer.Domain.Entities;

namespace ScadaServer.Infrastructure.Communication
{
    public interface IProtocolDriver : IAsyncDisposable
    {
        /// <summary>
        /// 连接设备
        /// </summary>
        /// <param name="device">设备实体</param>
        /// <param name="configJson">协议配置 JSON</param>
        Task<bool> ConnectAsync(Device device, string configJson);
        
        Task<object> ReadAsync(ModelVariable variable);
        Task<IDictionary<string, object>> ReadBatchAsync(IEnumerable<ModelVariable> variables);
        
        // 批量订阅接口
        Task SubscribeAsync(IEnumerable<ModelVariable> variables, Action<string, object> onValueChanged);
        Task UnsubscribeAsync(IEnumerable<ModelVariable> variables);
        
        Task DisconnectAsync();
    }
}
