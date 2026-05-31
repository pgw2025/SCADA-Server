using SqlSugar;

namespace ScadaServer.Domain.Entities
{
    /// <summary>
    /// 组态工程表
    /// </summary>
    [SugarTable("ScadaProjects")]
    public class ScadaProject
    {
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// 工程下的页面
        /// </summary>
        [Navigate(NavigateType.OneToMany, nameof(ScadaPage.ProjectId))]
        public List<ScadaPage> Pages { get; set; }
    }
}
