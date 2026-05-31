using SqlSugar;

namespace ScadaServer.Domain.Entities
{
    /// <summary>
    /// 运行时传感器/点位实例表 (可选，用于快速检索当前值)
    /// </summary>
    [SugarTable("Sensors")]
    public class Sensor
    {
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int Id { get; set; }
        
        public int DeviceId { get; set; }
        [Navigate(NavigateType.OneToOne, nameof(DeviceId))]
        public Device Device { get; set; }

        /// <summary>
        /// 变量 Key (对应 ModelVariable.Key)
        /// </summary>
        public string VariableKey { get; set; }
        
        public string Name { get; set; }
        public string Unit { get; set; }
        public double LastValue { get; set; }
        public DateTime LastUpdateTime { get; set; }
    }
}
