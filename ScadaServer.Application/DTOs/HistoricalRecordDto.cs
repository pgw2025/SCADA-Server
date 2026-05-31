namespace ScadaServer.Application.DTOs
{
    public class HistoricalRecordDto
    {
        public long Id { get; set; }
        public int DeviceId { get; set; }
        public string VariableKey { get; set; }
        public string Value { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
