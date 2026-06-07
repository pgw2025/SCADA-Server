namespace ScadaServer.Domain.Entities
{
    public class HistoricalRecord
    {
        public long Id { get; set; }
        public int DeviceId { get; set; }
        public string VariableKey { get; set; }
        public string Value { get; set; }
        public DateTime Timestamp { get; set; }
    }
}