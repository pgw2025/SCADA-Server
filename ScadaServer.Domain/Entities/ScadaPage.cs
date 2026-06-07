using SqlSugar;

namespace ScadaServer.Domain.Entities
{
    [SugarTable("ScadaPages")]
        public class ScadaPage : EntityBase
        {
            public int ProjectId { get; set; }
            [Navigate(NavigateType.OneToOne, nameof(ProjectId))]
            public ScadaProject Project { get; set; }

            public string Name { get; set; }
            public bool IsHome { get; set; }

            [SugarColumn(IsIgnore = true)]
            public List<HmiComponent> Components { get; set; }
        }
}