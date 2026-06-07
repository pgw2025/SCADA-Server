using SqlSugar;

namespace ScadaServer.Infrastructure.DBEntities
{
    [SugarTable("RealtimeData")]
    public class RealtimeDataDbEntity
    {
        [SugarColumn(IsPrimaryKey = true)]
        public int DeviceId { get; set; }
        [SugarColumn(IsPrimaryKey = true)]
        public string VariableKey { get; set; }
        public string Value { get; set; }
        public DateTime Timestamp { get; set; }
    }
}