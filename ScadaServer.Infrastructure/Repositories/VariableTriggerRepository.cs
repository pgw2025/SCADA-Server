using SqlSugar;
using ScadaServer.Domain.Entities;
using ScadaServer.Infrastructure.Persistence;

namespace ScadaServer.Infrastructure.Repositories
{
    public class VariableTriggerRepository : SqlSugarRepository<VariableTrigger> { public VariableTriggerRepository(ISqlSugarClient db) : base(db) { } }
}
