using ScadaServer.Application.Interfaces;
using SqlSugar;
using ScadaServer.Domain.Entities;
using ScadaServer.Infrastructure.Persistence;

namespace ScadaServer.Infrastructure.Repositories
{
    public class DataModelRepository : SqlSugarRepository<DataModel>, IDataModelRepository
    {
        public DataModelRepository(ISqlSugarClient db) : base(db) { }

        public override async Task<DataModel> GetByIdAsync(dynamic id)
        {
            return await _db.Queryable<DataModel>()
                            .Includes(x => x.Variables)
                            .In(id)
                            .FirstAsync();
        }

        public override async Task<List<DataModel>> GetListAsync()
        {
            return await _db.Queryable<DataModel>()
                            .Includes(x => x.Variables)
                            .ToListAsync();
        }
    }
}
