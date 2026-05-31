using SqlSugar;

namespace ScadaServer.Domain.Entities
{
    /// <summary>
    /// 全局配置表
    /// </summary>
    [SugarTable("SystemConfig")]
    public class SystemConfig
    {
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int Id { get; set; }
        public string SystemTitle { get; set; }
        public int PollIntervalMs { get; set; }
        public string MqttBrokerHost { get; set; }
        public int RetentionPeriodDays { get; set; }
    }
}
