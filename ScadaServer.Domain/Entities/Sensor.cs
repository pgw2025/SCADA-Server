using SqlSugar;

namespace ScadaServer.Domain.Entities
{
    [SugarTable("Sensors")]
    public class Sensor : EntityBase
    {
        public int DeviceId { get; set; }
        [Navigate(NavigateType.OneToOne, nameof(DeviceId))]
        public Device Device { get; set; }

        public string VariableKey { get; set; }
        
        public string Name { get; set; }
        public string Unit { get; set; }
        public double LastValue { get; set; }
        public DateTime LastUpdateTime { get; set; }
    }
}