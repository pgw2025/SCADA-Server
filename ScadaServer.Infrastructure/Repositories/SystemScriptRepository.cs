using SqlSugar;
using ScadaServer.Domain.Entities;
using ScadaServer.Infrastructure.Persistence;

namespace ScadaServer.Infrastructure.Repositories
{
    public class SystemScriptRepository : SqlSugarRepository<SystemScript> { public SystemScriptRepository(ISqlSugarClient db) : base(db) { } }
}
