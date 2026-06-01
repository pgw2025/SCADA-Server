using ScadaServer.Application.Interfaces;
using SqlSugar; using ScadaServer.Domain.Entities; using ScadaServer.Infrastructure.Persistence;  namespace ScadaServer.Infrastructure.Repositories {     public class SystemConfigRepository : SqlSugarRepository<SystemConfig>, ISystemConfigRepository { public SystemConfigRepository(ISqlSugarClient db) : base(db) { } } }
