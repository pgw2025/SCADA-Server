using SqlSugar;

namespace ScadaServer.Domain.Entities
{
    /// <summary>
    /// 系统脚本表 - 处理设备间的联动、自定义逻辑
    /// </summary>
    [SugarTable("SystemScripts")]
    public class SystemScript
    {
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int Id { get; set; }
        public string Name { get; set; }
        /// <summary>
        /// JavaScript 或 C# 内容
        /// </summary>
        [SugarColumn(ColumnDataType = "text")]
        public string Code { get; set; }
        /// <summary>
        /// 触发类型：Auto/Manual/Event
        /// </summary>
        public string TriggerType { get; set; }
        public int? IntervalSeconds { get; set; }
        public bool Active { get; set; }
    }
}
