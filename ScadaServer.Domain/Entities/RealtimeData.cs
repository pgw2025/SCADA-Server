using SqlSugar;

namespace ScadaServer.Domain.Entities
{
    /// <summary>
    /// 实时数据快照表 - 建议使用缓存，物理表用于持久化快照
    /// </summary>
    [SugarTable("RealtimeData")]
    public class RealtimeData
    {
        [SugarColumn(IsPrimaryKey = true)]
        public int DeviceId { get; set; }
        [SugarColumn(IsPrimaryKey = true)]
        public string VariableKey { get; set; }
        public string Value { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
