using ScadaServer.Application.Interfaces;
using SqlSugar; using ScadaServer.Domain.Entities; using ScadaServer.Infrastructure.Persistence;  namespace ScadaServer.Infrastructure.Repositories {     public class SystemScriptRepository : SqlSugarRepository<SystemScript>, ISystemScriptRepository { public SystemScriptRepository(ISqlSugarClient db) : base(db) { } } }
