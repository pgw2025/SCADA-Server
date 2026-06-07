using SqlSugar;
using ScadaServer.Domain.Enums;

namespace ScadaServer.Domain.Entities
{
    /// <summary>
    /// 变量模型表 - 定义一类设备的变量结构（模板）
    /// </summary>
    [SugarTable("DataModels")]
    public class DataModel
    {
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int Id { get; set; }

        [SugarColumn(Length = 100, IsNullable = false)]
        public string Name { get; set; } = string.Empty;

        [SugarColumn(Length = 500)]
        public string? Description { get; set; }

        /// <summary>
        /// 设备类型（枚举）
        /// </summary>
        public DeviceType Type { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        /// <summary>
        /// 模型下的变量列表
        /// </summary>
        [Navigate(NavigateType.OneToMany, nameof(ModelVariable.ModelId))]
        public List<ModelVariable>? Variables { get; set; }
    }
}
