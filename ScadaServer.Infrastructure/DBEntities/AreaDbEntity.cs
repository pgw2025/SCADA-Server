using SqlSugar;

namespace ScadaServer.Infrastructure.DBEntities
{
    /// <summary>
    /// 区域数据库实体
    /// </summary>
    [SugarTable("Areas")]
    public class AreaDbEntity
    {
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
}