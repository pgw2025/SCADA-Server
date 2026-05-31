using SqlSugar;

namespace ScadaServer.Domain.Entities
{
    /// <summary>
    /// 模型变量定义表 - 定义模型下的具体点位
    /// </summary>
    [SugarTable("ModelVariables")]
    public class ModelVariable
    {
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int Id { get; set; }
        public int ModelId { get; set; }
        /// <summary>
        /// 变量标识（如 pump_speed）
        /// </summary>
        public string Key { get; set; }
        public string Name { get; set; }
        /// <summary>
        /// 类型：Analog/Digital
        /// </summary>
        public string Type { get; set; }
        /// <summary>
        /// 数据类型：INT/REAL/BOOL
        /// </summary>
        public string DataType { get; set; }
        public string Unit { get; set; }
        public double? Min { get; set; }
        public double? Max { get; set; }
        /// <summary>
        /// 寄存器地址
        /// </summary>
        public string Address { get; set; }
        public string Description { get; set; }
        /// <summary>
        /// 是否存入历史库
        /// </summary>
        public bool IsStored { get; set; }
        /// <summary>
        /// 存储模式：变化存储/周期存储
        /// </summary>
        public string StoreMode { get; set; }
    }
}
