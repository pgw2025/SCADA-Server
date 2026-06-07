using SqlSugar;

namespace ScadaServer.Domain.Entities
{
    [SugarTable("SystemConfig")]
    public class SystemConfig : EntityBase
    {
        public string SystemTitle { get; set; }
        public int PollIntervalMs { get; set; }
        public string MqttBrokerHost { get; set; }
        public int RetentionPeriodDays { get; set; }
    }
}