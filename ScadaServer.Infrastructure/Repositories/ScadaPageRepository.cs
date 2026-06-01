using ScadaServer.Application.Interfaces;
using SqlSugar; using ScadaServer.Domain.Entities; using ScadaServer.Infrastructure.Persistence;  namespace ScadaServer.Infrastructure.Repositories {     public class ScadaPageRepository : SqlSugarRepository<ScadaPage>, IScadaPageRepository { public ScadaPageRepository(ISqlSugarClient db) : base(db) { } } }
