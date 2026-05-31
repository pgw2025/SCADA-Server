using SqlSugar;
using ScadaServer.Domain.Entities;
using ScadaServer.Infrastructure.Persistence;

namespace ScadaServer.Infrastructure.Repositories
{
    public class DatabaseConfigRepository : SqlSugarRepository<DatabaseConfig> { public DatabaseConfigRepository(ISqlSugarClient db) : base(db) { } }
}
