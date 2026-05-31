namespace ScadaServer.Application.Interfaces
{
    public interface IMqttService
    {
        Task PublishAsync(string topic, string payload);
        Task SubscribeAsync(string topic);
    }
}
