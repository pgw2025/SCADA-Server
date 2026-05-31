using SqlSugar;

namespace ScadaServer.Domain.Entities
{
    /// <summary>
    /// MQTT 服务器配置表
    /// </summary>
    [SugarTable("MqttServers")]
    public class MqttServer
    {
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int Id { get; set; }
        public string Name { get; set; }
        public string BrokerUrl { get; set; }
        public int Port { get; set; }
        public string ClientId { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string TopicPrefix { get; set; }
    }
}
