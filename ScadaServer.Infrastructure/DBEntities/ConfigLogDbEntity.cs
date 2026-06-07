using SqlSugar;

namespace ScadaServer.Infrastructure.DBEntities
{
    [SugarTable("ConfigLog")]
    public class ConfigLogDbEntity
    {
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int Id { get; set; }
        public int DeviceId { get; set; }
        public string Operator { get; set; }
        public string ChangeDesc { get; set; }
        public DateTime CreateTime { get; set; }
    }
}