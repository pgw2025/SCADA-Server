using ScadaServer.Application.Interfaces;
using SqlSugar; using ScadaServer.Domain.Entities; using ScadaServer.Infrastructure.Persistence;  namespace ScadaServer.Infrastructure.Repositories {     public class SystemUserRepository : SqlSugarRepository<SystemUser>, ISystemUserRepository { public SystemUserRepository(ISqlSugarClient db) : base(db) { } } }
