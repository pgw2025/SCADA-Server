using SqlSugar;

namespace ScadaServer.Domain.Entities
{
    /// <summary>
    /// 系统日志表
    /// </summary>
    [SugarTable("SystemLogs")]
    public class SystemLog
    {
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int Id { get; set; }
        public DateTime Timestamp { get; set; }
        /// <summary>
        /// 级别：Info/Warning/Error
        /// </summary>
        public string Level { get; set; }
        public string Source { get; set; }
        public string Content { get; set; }
    }
}
