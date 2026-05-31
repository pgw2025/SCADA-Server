using SqlSugar;

namespace ScadaServer.Domain.Entities
{
    /// <summary>
    /// 外部数据库配置表 - 支持多库存储
    /// </summary>
    [SugarTable("DatabaseConfigs")]
    public class DatabaseConfig
    {
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int Id { get; set; }
        public string Name { get; set; }
        /// <summary>
        /// 类型：Realtime/Historical
        /// </summary>
        public string Type { get; set; }
        /// <summary>
        /// 后端类型：MySQL/InfluxDB/PostgreSQL 等
        /// </summary>
        public string BackendType { get; set; }
        public string Host { get; set; }
        public int Port { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string DatabaseName { get; set; }
    }
}
