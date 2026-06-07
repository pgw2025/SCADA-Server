using SqlSugar;

namespace ScadaServer.Domain.Entities
{
    public abstract class EntityBase
    {
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int Id { get; set; }
    }
}