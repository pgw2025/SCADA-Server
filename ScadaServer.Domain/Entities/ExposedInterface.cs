using SqlSugar;

namespace ScadaServer.Domain.Entities
{
    [SugarTable("ExposedInterfaces")]
    public class ExposedInterface : EntityBase
    {
        public string Name { get; set; }
        public string RouteUrl { get; set; }
        public string RequestMethod { get; set; }

        public int DeviceId { get; set; }
        [Navigate(NavigateType.OneToOne, nameof(DeviceId))]
        public Device Device { get; set; }

        public string ExposedKey { get; set; }
        public bool Active { get; set; }
    }
}