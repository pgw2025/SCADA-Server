using SqlSugar;

namespace ScadaServer.Infrastructure.DBEntities
{
    [SugarTable("ScheduledTasks")]
    public class ScheduledTaskDbEntity
    {
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string CronExpression { get; set; }
        [SugarColumn(ColumnDataType = "text")]
        public string ParamsJson { get; set; }
        public bool Active { get; set; }
    }
}