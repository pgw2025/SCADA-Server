using SqlSugar;

namespace ScadaServer.Infrastructure.DBEntities
{
    [SugarTable("ScadaPages")]
    public class ScadaPageDbEntity
    {
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int Id { get; set; }
        
        public int ProjectId { get; set; }
        [Navigate(NavigateType.OneToOne, nameof(ProjectId))]
        public ScadaProjectDbEntity Project { get; set; }

        public string Name { get; set; }
        public bool IsHome { get; set; }
    }
}