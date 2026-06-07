using SqlSugar;

namespace ScadaServer.Infrastructure.DBEntities
{
    [SugarTable("MqttServers")]
    public class MqttServerDbEntity
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