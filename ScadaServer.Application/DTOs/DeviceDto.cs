using ScadaServer.Domain.Enums;

namespace ScadaServer.Application.DTOs
{
    public class DeviceDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public int AreaId { get; set; }
        public int ModelId { get; set; }
        public string Type { get; set; }
        public string IpAddress { get; set; }
        public int? Port { get; set; }
        public string Topic { get; set; }
        public DeviceStatus Status { get; set; }
        public string CpuType { get; set; }
        public int? Rack { get; set; }
        public int? Slot { get; set; }
        public DateTime LastUpdated { get; set; }
    }
}
