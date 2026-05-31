using SqlSugar;
using ScadaServer.Domain.Entities;
using ScadaServer.Infrastructure.Persistence;

namespace ScadaServer.Infrastructure.Repositories
{
    public class ConfigLogRepository : SqlSugarRepository<ConfigLog> { public ConfigLogRepository(ISqlSugarClient db) : base(db) { } }
}
