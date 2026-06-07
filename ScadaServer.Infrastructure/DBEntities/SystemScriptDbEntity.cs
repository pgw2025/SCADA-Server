using SqlSugar;

namespace ScadaServer.Infrastructure.DBEntities
{
    [SugarTable("SystemScripts")]
    public class SystemScriptDbEntity
    {
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int Id { get; set; }
        public string Name { get; set; }
        [SugarColumn(ColumnDataType = "text")]
        public string Code { get; set; }
        public string TriggerType { get; set; }
        public int? IntervalSeconds { get; set; }
        public bool Active { get; set; }
    }
}