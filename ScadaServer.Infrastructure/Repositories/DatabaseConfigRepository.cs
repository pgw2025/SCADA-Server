using ScadaServer.Application.Interfaces;
using SqlSugar; using ScadaServer.Domain.Entities; using ScadaServer.Infrastructure.Persistence;  namespace ScadaServer.Infrastructure.Repositories {     public class DatabaseConfigRepository : SqlSugarRepository<DatabaseConfig>, IDatabaseConfigRepository { public DatabaseConfigRepository(ISqlSugarClient db) : base(db) { } } }
