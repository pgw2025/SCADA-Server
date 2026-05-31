using SqlSugar;
using ScadaServer.Domain.Entities;
using ScadaServer.Infrastructure.Persistence;

namespace ScadaServer.Infrastructure.Repositories
{
    public class DataModelRepository : SqlSugarRepository<DataModel> { public DataModelRepository(ISqlSugarClient db) : base(db) { } }
}
