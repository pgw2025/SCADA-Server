using SqlSugar;

namespace ScadaServer.Infrastructure.DBEntities
{
    [SugarTable("Sensors")]
    public class SensorDbEntity
    {
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int Id { get; set; }
        
        public int DeviceId { get; set; }
        [Navigate(NavigateType.OneToOne, nameof(DeviceId))]
        public DeviceDbEntity Device { get; set; }

        public string VariableKey { get; set; }
        
        public string Name { get; set; }
        public string Unit { get; set; }
        public double LastValue { get; set; }
        public DateTime LastUpdateTime { get; set; }
    }
}