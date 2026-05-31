using SqlSugar;
using ScadaServer.Domain.Entities;
using ScadaServer.Infrastructure.Persistence;

namespace ScadaServer.Infrastructure.Repositories
{
    public class HmiComponentRepository : SqlSugarRepository<HmiComponent> { public HmiComponentRepository(ISqlSugarClient db) : base(db) { } }
}
