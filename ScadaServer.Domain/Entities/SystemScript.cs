using SqlSugar;

namespace ScadaServer.Domain.Entities
{
    [SugarTable("SystemScripts")]
    public class SystemScript : EntityBase
    {
        public string Name { get; set; }
        [SugarColumn(ColumnDataType = "text")]
        public string Code { get; set; }
        public string TriggerType { get; set; }
        public int? IntervalSeconds { get; set; }
        public bool Active { get; set; }
    }
}