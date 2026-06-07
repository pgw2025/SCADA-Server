using SqlSugar;

namespace ScadaServer.Infrastructure.DBEntities
{
    [SugarTable("SystemLogs")]
    public class SystemLogDbEntity
    {
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int Id { get; set; }
        public DateTime Timestamp { get; set; }
        public string Level { get; set; }
        public string Source { get; set; }
        public string Content { get; set; }
    }
}