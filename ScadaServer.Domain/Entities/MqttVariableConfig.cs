using SqlSugar;

namespace ScadaServer.Domain.Entities
{
    /// <summary>
    /// MQTT 变量发布配置表 - 一个变量可以关联多个 MQTT 服务器并设置别名
    /// </summary>
    [SugarTable("MqttVariableConfigs")]
    public class MqttVariableConfig
    {
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int Id { get; set; }
        
        /// <summary>
        /// 关联的 MQTT 服务器 ID
        /// </summary>
        public int MqttServerId { get; set; }
        
        /// <summary>
        /// 关联的设备 ID
        /// </summary>
        public int DeviceId { get; set; }
        
        /// <summary>
        /// 关联的变量标识 (ModelVariable.Key)
        /// </summary>
        public string VariableKey { get; set; }
        
        /// <summary>
        /// 该变量在此 MQTT 服务器上的别名 (发送时使用)
        /// </summary>
        public string Alias { get; set; }
        
        /// <summary>
        /// 是否启用发布
        /// </summary>
        public bool IsEnabled { get; set; } = true;

        /// <summary>
        /// 可选：覆盖全局的主题规则，如果为空则使用 服务器前缀/别名
        /// </summary>
        public string? CustomTopic { get; set; }
    }
}
