using SqlSugar;
using ScadaServer.Domain.Entities;
using ScadaServer.Infrastructure.Persistence;

namespace ScadaServer.Infrastructure.Repositories
{
    public class SystemConfigRepository : SqlSugarRepository<SystemConfig> { public SystemConfigRepository(ISqlSugarClient db) : base(db) { } }
}
