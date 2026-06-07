using SqlSugar;

namespace ScadaServer.Infrastructure.DBEntities
{
    /// <summary>
    /// 变量触发器数据库实体
    /// </summary>
    [SugarTable("VariableTriggers")]
    public class VariableTriggerDbEntity
    {
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int Id { get; set; }
        public string Name { get; set; }
        public int DeviceId { get; set; }
        public string VariableKey { get; set; }
        public string Condition { get; set; }
        public double Threshold { get; set; }
        public string ActionType { get; set; }
        public string AlarmLevel { get; set; }
        public string LinkageVariableKey { get; set; }
        public string LinkageValue { get; set; }
        public bool Active { get; set; }
    }
}