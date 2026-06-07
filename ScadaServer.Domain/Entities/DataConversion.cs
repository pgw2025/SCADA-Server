using SqlSugar;

namespace ScadaServer.Domain.Entities
{
    [SugarTable("DataConversions")]
    public class DataConversion : EntityBase
    {
        public string Name { get; set; }
        public int SourceDeviceId { get; set; }
        public string SourceVariableKey { get; set; }
        public int TargetDeviceId { get; set; }
        public string TargetVariableKey { get; set; }
        public bool Active { get; set; }
    }
}