using SqlSugar;

namespace ScadaServer.Domain.Entities
{
    /// <summary>
    /// 数据联动/转换表 - 实现“变量 A 改变，自动写入变量 B”的逻辑
    /// </summary>
    [SugarTable("DataConversions")]
    public class DataConversion
    {
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int Id { get; set; }
        public string Name { get; set; }
        public int SourceDeviceId { get; set; }
        public string SourceVariableKey { get; set; }
        public int TargetDeviceId { get; set; }
        public string TargetVariableKey { get; set; }
        public bool Active { get; set; }
    }
}
