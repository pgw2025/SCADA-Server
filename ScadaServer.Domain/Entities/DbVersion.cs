using SqlSugar;

namespace ScadaServer.Domain.Entities
{
    [SugarTable("DbVersion")]
    public class DbVersion : EntityBase
    {

        public string Version { get; set; } = "";

        public DateTime AppliedAt { get; set; } = DateTime.Now;
    }
}