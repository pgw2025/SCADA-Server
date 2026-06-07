using SqlSugar;

namespace ScadaServer.Infrastructure.DBEntities
{
    [SugarTable("ExposedInterfaces")]
    public class ExposedInterfaceDbEntity
    {
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int Id { get; set; }
        public string Name { get; set; }
        public string RouteUrl { get; set; }
        public string RequestMethod { get; set; }
        
        public int DeviceId { get; set; }
        [Navigate(NavigateType.OneToOne, nameof(DeviceId))]
        public DeviceDbEntity Device { get; set; }

        public string ExposedKey { get; set; }
        public bool Active { get; set; }
    }
}