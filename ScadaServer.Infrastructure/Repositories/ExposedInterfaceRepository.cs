using ScadaServer.Application.Interfaces;
using SqlSugar; using ScadaServer.Domain.Entities; using ScadaServer.Infrastructure.Persistence;  namespace ScadaServer.Infrastructure.Repositories {     public class ExposedInterfaceRepository : SqlSugarRepository<ExposedInterface>, IExposedInterfaceRepository { public ExposedInterfaceRepository(ISqlSugarClient db) : base(db) { } } }
