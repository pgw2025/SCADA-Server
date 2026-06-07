using SqlSugar;

namespace ScadaServer.Domain.Entities
{
    [SugarTable("SystemLogs")]
    public class SystemLog : EntityBase
    {
        public DateTime Timestamp { get; set; }
        public string Level { get; set; }
        public string Source { get; set; }
        public string Content { get; set; }
    }
}