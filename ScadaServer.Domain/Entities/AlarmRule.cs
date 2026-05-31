using SqlSugar;

namespace ScadaServer.Domain.Entities
{
    [SugarTable("AlarmRules")]
    public class AlarmRule
    {
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int Id { get; set; }
        public int SensorId { get; set; }
        public string Condition { get; set; } // e.g., ">", "<"
        public double Threshold { get; set; }
        public string Severity { get; set; } // Info, Warning, Critical
        public bool IsEnabled { get; set; }
    }
}
