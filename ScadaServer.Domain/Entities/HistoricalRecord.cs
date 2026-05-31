using SqlSugar;

namespace ScadaServer.Domain.Entities
{
    /// <summary>
    /// 历史采样表 - 建议使用时序数据库
    /// </summary>
    [SugarTable("HistoricalRecords")]
    public class HistoricalRecord
    {
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public long Id { get; set; }
        public int DeviceId { get; set; }
        public string VariableKey { get; set; }
        public string Value { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
