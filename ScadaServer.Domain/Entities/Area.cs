using SqlSugar;

namespace ScadaServer.Domain.Entities
{
    /// <summary>
    /// 区域表 - 用于划分厂区、车间等
    /// </summary>
    [SugarTable("Areas")]
    public class Area
    {
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
