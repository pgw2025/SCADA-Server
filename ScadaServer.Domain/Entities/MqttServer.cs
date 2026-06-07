using SqlSugar;

namespace ScadaServer.Domain.Entities
{
    [SugarTable("MqttServers")]
    public class MqttServer : EntityBase
    {
        public string Name { get; set; }
        public string BrokerUrl { get; set; }
        public int Port { get; set; }
        public string ClientId { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string TopicPrefix { get; set; }
    }
}