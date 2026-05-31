using SqlSugar;

namespace ScadaServer.Domain.Entities
{
    /// <summary>
    /// 变量触发器表 - 处理告警、联动触发逻辑
    /// </summary>
    [SugarTable("VariableTriggers")]
    public class VariableTrigger
    {
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int Id { get; set; }
        public string Name { get; set; }
        public int DeviceId { get; set; }
        public string VariableKey { get; set; }
        /// <summary>
        /// 条件：>, <, =, !=, >=, <=
        /// </summary>
        public string Condition { get; set; }
        public double Threshold { get; set; }
        /// <summary>
        /// 动作类型：Alarm/Linkage
        /// </summary>
        public string ActionType { get; set; }
        /// <summary>
        /// 告警级别：Info/Warning/Critical
        /// </summary>
        public string AlarmLevel { get; set; }
        /// <summary>
        /// 联动变量 Key
        /// </summary>
        public string LinkageVariableKey { get; set; }
        /// <summary>
        /// 联动写入值
        /// </summary>
        public string LinkageValue { get; set; }
        public bool Active { get; set; }
    }
}
