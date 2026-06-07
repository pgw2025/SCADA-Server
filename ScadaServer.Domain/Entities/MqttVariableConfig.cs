using SqlSugar;

namespace ScadaServer.Domain.Entities
{
    [SugarTable("MqttVariableConfigs")]
        public class MqttVariableConfig : EntityBase
        {
            public int MqttServerId { get; set; }

            public int DeviceId { get; set; }

            public string VariableKey { get; set; }

            public string Alias { get; set; }

            public bool IsEnabled { get; set; } = true;

            public string? CustomTopic { get; set; }
        }
}