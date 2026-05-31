using ScadaServer.Application.Interfaces;
using MQTTnet;
using MQTTnet.Client;

namespace ScadaServer.Infrastructure.Communication
{
    public class MqttHandler : IMqttService
    {
        private readonly IMqttClient _mqttClient;

        public MqttHandler()
        {
            var factory = new MqttFactory();
            _mqttClient = factory.CreateMqttClient();
        }

        public async Task PublishAsync(string topic, string payload)
        {
            var message = new MqttApplicationMessageBuilder()
                .WithTopic(topic)
                .WithPayload(payload)
                .Build();

            if (!_mqttClient.IsConnected) 
            {
                // In real implementation, handle connection options from config
            }
            
            await _mqttClient.PublishAsync(message);
        }

        public async Task SubscribeAsync(string topic)
        {
            // Implementation for subscribing to topics
            await Task.CompletedTask;
        }
    }
}
