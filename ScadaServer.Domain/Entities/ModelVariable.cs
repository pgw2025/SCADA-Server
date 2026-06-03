using SqlSugar;
using ScadaServer.Domain.Enums;

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
        
        public VariableType Type { get; set; }
        public DataTypeEnum DataType { get; set; }
        
        public string Unit { get; set; }
        public double? Min { get; set; }
        public double? Max { get; set; }
        /// <summary>
        /// 寄存器地址
        /// </summary>
        public string Address { get; set; }
        public string? Description { get; set; }
        /// <summary>
        /// 是否存入历史库
        /// </summary>
        public bool IsStored { get; set; }
        /// <summary>
        /// 存储模式：变化存储/周期存储
        /// </summary>
        public string StoreMode { get; set; }

        public UpdateMode UpdateMode { get; set; }
        public int PollingIntervalMs { get; set; } = 1000;

        // --- 工业级增强字段 ---
        
        /// <summary>
        /// 位偏移 (用于 S7/Modbus 的位寻址，如 DB1.DBX0.6 中的 6)
        /// </summary>
        public int? BitOffset { get; set; }

        /// <summary>
        /// 缩放斜率 (显示值 = 原始值 * ScaleSlope + ScaleOffset)
        /// </summary>
        public double ScaleSlope { get; set; } = 1.0;

        /// <summary>
        /// 缩放偏移
        /// </summary>
        public double ScaleOffset { get; set; } = 0.0;

        /// <summary>
        /// 采集死区 (模拟量波动小于此值时不更新/不存储)
        /// </summary>
        public double? DeadBand { get; set; }

        /// <summary>
        /// 是否只读 (防止对传感器等变量执行写入操作)
        /// </summary>
        public bool IsReadOnly { get; set; } = true;

        /// <summary>
        /// 扩展属性 JSON (用于存放非核心配置，如 UI 配置、私有协议参数等)
        /// </summary>
        [SugarColumn(ColumnDataType = "longtext", IsJson = true)]
        public Dictionary<string, string>? ExtensionData { get; set; }
    }
}
