using ScadaServer.Application.Interfaces;
using SqlSugar; using ScadaServer.Domain.Entities; using ScadaServer.Infrastructure.Persistence;  namespace ScadaServer.Infrastructure.Repositories {     public class SystemLogRepository : SqlSugarRepository<SystemLog>, ISystemLogRepository { public SystemLogRepository(ISqlSugarClient db) : base(db) { } } }
