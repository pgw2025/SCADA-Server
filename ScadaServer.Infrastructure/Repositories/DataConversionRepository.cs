using SqlSugar;
using ScadaServer.Domain.Entities;
using ScadaServer.Infrastructure.Persistence;

namespace ScadaServer.Infrastructure.Repositories
{
    public class DataConversionRepository : SqlSugarRepository<DataConversion> { public DataConversionRepository(ISqlSugarClient db) : base(db) { } }
}
