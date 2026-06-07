namespace ScadaServer.Domain.Entities
{
    public class RealtimeData
    {
        public int DeviceId { get; set; }
        public string VariableKey { get; set; }
        public string Value { get; set; }
        public DateTime Timestamp { get; set; }
    }
}