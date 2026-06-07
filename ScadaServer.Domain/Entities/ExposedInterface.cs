namespace ScadaServer.Domain.Entities
{
    public class ExposedInterface
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string RouteUrl { get; set; }
        public string RequestMethod { get; set; }
        
        public int DeviceId { get; set; }
        public Device Device { get; set; }

        public string ExposedKey { get; set; }
        public bool Active { get; set; }
    }
}