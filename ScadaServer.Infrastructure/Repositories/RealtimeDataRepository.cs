using SqlSugar;
using ScadaServer.Domain.Entities;
using ScadaServer.Infrastructure.Persistence;

namespace ScadaServer.Infrastructure.Repositories
{
    public class RealtimeDataRepository : SqlSugarRepository<RealtimeData> { public RealtimeDataRepository(ISqlSugarClient db) : base(db) { } }
}
