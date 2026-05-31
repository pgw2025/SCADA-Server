using SqlSugar;
using ScadaServer.Domain.Enums;

namespace ScadaServer.Domain.Entities
{
    /// <summary>
    /// 设备表 - 物理设备的实例
    /// </summary>
    [SugarTable("Devices")]
    public class Device
    {
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int Id { get; set; }
        public string Name { get; set; }
        /// <summary>
        /// 唯一编码符
        /// </summary>
        [SugarColumn(ColumnDescription = "唯一编码符")]
        public string Code { get; set; }
        
        public int AreaId { get; set; }
        /// <summary>
        /// 关联区域
        /// </summary>
        [Navigate(NavigateType.OneToOne, nameof(AreaId))]
        public Area Area { get; set; }

        public int ModelId { get; set; }
        /// <summary>
        /// 关联变量模型
        /// </summary>
        [Navigate(NavigateType.OneToOne, nameof(ModelId))]
        public DataModel Model { get; set; }

        /// <summary>
        /// 类型：OPCUA/S7/MQTT 等
        /// </summary>
        public string Type { get; set; }
        public string IpAddress { get; set; }
        public int? Port { get; set; }
        /// <summary>
        /// MQTT主路径
        /// </summary>
        public string Topic { get; set; }
        public DeviceStatus Status { get; set; }
        
        // S7 协议特定
        public string CpuType { get; set; }
        public int? Rack { get; set; }
        public int? Slot { get; set; }

        public DateTime LastUpdated { get; set; }

        /// <summary>
        /// 该设备下的触发器
        /// </summary>
        [Navigate(NavigateType.OneToMany, nameof(VariableTrigger.DeviceId))]
        public List<VariableTrigger> Triggers { get; set; }
    }
}
