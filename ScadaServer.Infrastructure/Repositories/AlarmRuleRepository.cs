using SqlSugar;
using ScadaServer.Domain.Entities;
using ScadaServer.Infrastructure.Persistence;

namespace ScadaServer.Infrastructure.Repositories
{
    public class AlarmRuleRepository : SqlSugarRepository<AlarmRule> { public AlarmRuleRepository(ISqlSugarClient db) : base(db) { } }
}
