namespace ScadaServer.Application.DTOs
{
    public class RealtimeDataDto
    {
        public int DeviceId { get; set; }
        public string VariableKey { get; set; }
        public string Value { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
