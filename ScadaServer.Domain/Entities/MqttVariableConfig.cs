namespace ScadaServer.Domain.Entities
{
    public class MqttVariableConfig
    {
        public int Id { get; set; }
        public int MqttServerId { get; set; }
        public int DeviceId { get; set; }
        public string VariableKey { get; set; }
        public string Alias { get; set; }
        public bool IsEnabled { get; set; } = true;
        public string? CustomTopic { get; set; }
    }
}