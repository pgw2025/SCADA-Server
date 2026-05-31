using SqlSugar;
using ScadaServer.Domain.Entities;
using ScadaServer.Infrastructure.Persistence;

namespace ScadaServer.Infrastructure.Repositories
{
    public class HistoricalRecordRepository : SqlSugarRepository<HistoricalRecord> { public HistoricalRecordRepository(ISqlSugarClient db) : base(db) { } }
}
