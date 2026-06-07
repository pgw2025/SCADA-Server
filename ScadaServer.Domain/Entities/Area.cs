using SqlSugar;

namespace ScadaServer.Domain.Entities
{
    [SugarTable("Areas")]
    public class Area : EntityBase
    {
        public string Name { get; set; }
        public string Description { get; set; }
    }
}