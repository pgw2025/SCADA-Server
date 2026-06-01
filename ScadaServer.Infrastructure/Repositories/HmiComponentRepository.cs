using ScadaServer.Application.Interfaces;
using SqlSugar; using ScadaServer.Domain.Entities; using ScadaServer.Infrastructure.Persistence;  namespace ScadaServer.Infrastructure.Repositories {     public class HmiComponentRepository : SqlSugarRepository<HmiComponent>, IHmiComponentRepository { public HmiComponentRepository(ISqlSugarClient db) : base(db) { } } }
