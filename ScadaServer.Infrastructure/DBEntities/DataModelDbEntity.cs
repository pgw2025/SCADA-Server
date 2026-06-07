using SqlSugar;
using ScadaServer.Domain.Enums;

namespace ScadaServer.Infrastructure.DBEntities
{
    /// <summary>
    /// 变量模型数据库实体
    /// </summary>
    [SugarTable("DataModels")]
    public class DataModelDbEntity
    {
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int Id { get; set; }

        [SugarColumn(Length = 100, IsNullable = false)]
        public string Name { get; set; } = string.Empty;

        [SugarColumn(Length = 500)]
        public string? Description { get; set; }

        public DeviceType Type { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }
}