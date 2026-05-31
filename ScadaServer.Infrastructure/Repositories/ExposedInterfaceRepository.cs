using SqlSugar;
using ScadaServer.Domain.Entities;
using ScadaServer.Infrastructure.Persistence;

namespace ScadaServer.Infrastructure.Repositories
{
    public class ExposedInterfaceRepository : SqlSugarRepository<ExposedInterface> { public ExposedInterfaceRepository(ISqlSugarClient db) : base(db) { } }
}
