using SqlSugar;

namespace ScadaServer.Infrastructure.DBEntities
{
    [SugarTable("ScadaProjects")]
    public class ScadaProjectDbEntity
    {
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}