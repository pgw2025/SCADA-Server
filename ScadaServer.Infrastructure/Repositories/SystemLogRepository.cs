using SqlSugar;
using ScadaServer.Domain.Entities;
using ScadaServer.Infrastructure.Persistence;

namespace ScadaServer.Infrastructure.Repositories
{
    public class SystemLogRepository : SqlSugarRepository<SystemLog> { public SystemLogRepository(ISqlSugarClient db) : base(db) { } }
}
