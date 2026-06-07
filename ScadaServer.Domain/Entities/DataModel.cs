using ScadaServer.Domain.Enums;

namespace ScadaServer.Domain.Entities
{
    public class DataModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DeviceType Type { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
        public List<ModelVariable>? Variables { get; set; }
    }
}