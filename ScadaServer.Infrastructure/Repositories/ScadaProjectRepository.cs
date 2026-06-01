using ScadaServer.Application.Interfaces;
using SqlSugar; using ScadaServer.Domain.Entities; using ScadaServer.Infrastructure.Persistence;  namespace ScadaServer.Infrastructure.Repositories {     public class ScadaProjectRepository : SqlSugarRepository<ScadaProject>, IScadaProjectRepository { public ScadaProjectRepository(ISqlSugarClient db) : base(db) { } } }
