using SqlSugar;
using ScadaServer.Domain.Entities;
using ScadaServer.Infrastructure.Persistence;

namespace ScadaServer.Infrastructure.Repositories
{
    public class ScheduledTaskRepository : SqlSugarRepository<ScheduledTask> { public ScheduledTaskRepository(ISqlSugarClient db) : base(db) { } }
}
