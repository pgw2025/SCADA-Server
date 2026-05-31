using SqlSugar;
using ScadaServer.Domain.Entities;
using ScadaServer.Infrastructure.Persistence;

namespace ScadaServer.Infrastructure.Repositories
{
    public class SystemUserRepository : SqlSugarRepository<SystemUser> { public SystemUserRepository(ISqlSugarClient db) : base(db) { } }
}
