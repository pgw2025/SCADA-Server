using SqlSugar;

namespace ScadaServer.Domain.Entities
{
    [SugarTable("ScadaProjects")]
    public class ScadaProject : EntityBase
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; }

        [SugarColumn(IsIgnore = true)]
        public List<ScadaPage> Pages { get; set; }
    }
}