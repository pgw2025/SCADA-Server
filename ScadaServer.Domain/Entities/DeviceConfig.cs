using SqlSugar;

namespace ScadaServer.Domain.Entities
{
    /// <summary>
    /// 设备协议配置表 - 存储各协议的 JSON 配置
    /// </summary>
    [SugarTable("DeviceConfigs")]
    public class DeviceConfig
    {
        /// <summary>
        /// 设备ID（主键，与 Device 一对一）
        /// </summary>
        [SugarColumn(IsPrimaryKey = true)]
        public int DeviceId { get; set; }

        /// <summary>
        /// JSON 格式的协议配置
        /// S7: {"IpAddress":"192.168.1.10","Port":102,"Rack":0,"Slot":1,"CpuType":"S71500"}
        /// ModbusTcp: {"IpAddress":"192.168.1.20","Port":502,"UnitId":1}
        /// OpcUa: {"EndpointUrl":"opc.tcp://localhost:4840","SecurityPolicy":"None"}
        /// Mqtt: {"Broker":"tcp://localhost:1883","Topic":"scada/data"}
        /// </summary>
        [SugarColumn(ColumnDataType = "nvarchar(max)")]
        public string JsonConfig { get; set; } = string.Empty;

        /// <summary>
        /// 配置版本号（用于热更新检测）
        /// </summary>
        public int Version { get; set; } = 1;

        /// <summary>
        /// 配置更新时间
        /// </summary>
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }
}
