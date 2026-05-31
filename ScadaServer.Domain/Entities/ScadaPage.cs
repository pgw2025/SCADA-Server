using SqlSugar;

namespace ScadaServer.Domain.Entities
{
    /// <summary>
    /// 组态页面表
    /// </summary>
    [SugarTable("ScadaPages")]
    public class ScadaPage
    {
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int Id { get; set; }
        
        public int ProjectId { get; set; }
        /// <summary>
        /// 所属工程
        /// </summary>
        [Navigate(NavigateType.OneToOne, nameof(ProjectId))]
        public ScadaProject Project { get; set; }

        public string Name { get; set; }
        public bool IsHome { get; set; }

        /// <summary>
        /// 页面上的控件列表
        /// </summary>
        [Navigate(NavigateType.OneToMany, nameof(HmiComponent.PageId))]
        public List<HmiComponent> Components { get; set; }
    }
}
