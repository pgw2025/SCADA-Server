using SqlSugar;
using ScadaServer.Domain.Entities;
using ScadaServer.Infrastructure.Persistence;

namespace ScadaServer.Infrastructure.Repositories
{
    public class ScadaPageRepository : SqlSugarRepository<ScadaPage> { public ScadaPageRepository(ISqlSugarClient db) : base(db) { } }
}
