using SqlSugar;

namespace ScadaServer.Infrastructure.DBEntities
{
    [SugarTable("HistoricalRecords")]
    public class HistoricalRecordDbEntity
    {
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public long Id { get; set; }
        public int DeviceId { get; set; }
        public string VariableKey { get; set; }
        public string Value { get; set; }
        public DateTime Timestamp { get; set; }
    }
}