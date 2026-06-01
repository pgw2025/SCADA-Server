using ScadaServer.Application.Interfaces;
using SqlSugar; using ScadaServer.Domain.Entities; using ScadaServer.Infrastructure.Persistence;  namespace ScadaServer.Infrastructure.Repositories {     public class ScheduledTaskRepository : SqlSugarRepository<ScheduledTask>, IScheduledTaskRepository { public ScheduledTaskRepository(ISqlSugarClient db) : base(db) { } } }
