using SqlSugar;

namespace ScadaServer.Infrastructure.DBEntities
{
    [SugarTable("SystemConfig")]
    public class SystemConfigDbEntity
    {
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int Id { get; set; }
        public string SystemTitle { get; set; }
        public int PollIntervalMs { get; set; }
        public string MqttBrokerHost { get; set; }
        public int RetentionPeriodDays { get; set; }
    }
}