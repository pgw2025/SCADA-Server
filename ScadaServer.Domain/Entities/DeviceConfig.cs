namespace ScadaServer.Domain.Entities
{
    public class DeviceConfig
    {
        public int DeviceId { get; set; }
        public string JsonConfig { get; set; } = string.Empty;
        public int Version { get; set; } = 1;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }
}