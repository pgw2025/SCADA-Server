using SqlSugar;

namespace ScadaServer.Domain.Entities
{
    /// <summary>
    /// 定时任务表
    /// </summary>
    [SugarTable("ScheduledTasks")]
    public class ScheduledTask
    {
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int Id { get; set; }
        public string Name { get; set; }
        /// <summary>
        /// 任务类型：Backup/SetValue/Script
        /// </summary>
        public string Type { get; set; }
        public string CronExpression { get; set; }
        /// <summary>
        /// 任务参数 JSON
        /// </summary>
        [SugarColumn(ColumnDataType = "text")]
        public string ParamsJson { get; set; }
        public bool Active { get; set; }
    }
}
