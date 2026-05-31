namespace ScadaServer.Application.DTOs
{
    public class DeviceDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Status { get; set; }
    }

    public class UpdateDeviceStatusDto
    {
        public int DeviceId { get; set; }
        public string NewStatus { get; set; }
    }
}
