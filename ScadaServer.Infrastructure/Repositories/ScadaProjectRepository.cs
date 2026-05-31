using SqlSugar;
using ScadaServer.Domain.Entities;
using ScadaServer.Infrastructure.Persistence;

namespace ScadaServer.Infrastructure.Repositories
{
    public class ScadaProjectRepository : SqlSugarRepository<ScadaProject> { public ScadaProjectRepository(ISqlSugarClient db) : base(db) { } }
}
