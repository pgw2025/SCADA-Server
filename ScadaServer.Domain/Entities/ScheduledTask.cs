using SqlSugar;

namespace ScadaServer.Domain.Entities
{
    [SugarTable("ScheduledTasks")]
    public class ScheduledTask : EntityBase
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public string CronExpression { get; set; }
        [SugarColumn(ColumnDataType = "text")]
        public string ParamsJson { get; set; }
        public bool Active { get; set; }
    }
}