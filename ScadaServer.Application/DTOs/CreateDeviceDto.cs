using ScadaServer.Domain.Enums;

namespace ScadaServer.Application.DTOs
{
    public class CreateDeviceDto
    {
        public string Name { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public int AreaId { get; set; }
        public int ModelId { get; set; }
        public string Type { get; set; } = string.Empty;
        public string IpAddress { get; set; } = string.Empty;
        public int? Port { get; set; }
        public string Topic { get; set; } = string.Empty;
        public DeviceStatus Status { get; set; }
        public string CpuType { get; set; } = string.Empty;
        public int? Rack { get; set; }
        public int? Slot { get; set; }
    }
}
