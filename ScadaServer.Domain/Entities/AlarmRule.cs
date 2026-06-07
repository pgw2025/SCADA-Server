using SqlSugar;

namespace ScadaServer.Domain.Entities
{
    [SugarTable("AlarmRules")]
    public class AlarmRule : EntityBase
    {
        public int SensorId { get; set; }
        public string Condition { get; set; }
        public double Threshold { get; set; }
        public string Severity { get; set; }
        public bool IsEnabled { get; set; }
    }
}