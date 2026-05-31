using SqlSugar;
using ScadaServer.Domain.Entities;
using ScadaServer.Infrastructure.Persistence;

namespace ScadaServer.Infrastructure.Repositories
{
    public class AreaRepository : SqlSugarRepository<Area> { public AreaRepository(ISqlSugarClient db) : base(db) { } }
}
