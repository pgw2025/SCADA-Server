using SqlSugar;
using ScadaServer.Domain.Entities;
using ScadaServer.Infrastructure.Persistence;

namespace ScadaServer.Infrastructure.Repositories
{
    public class ModelVariableRepository : SqlSugarRepository<ModelVariable> { public ModelVariableRepository(ISqlSugarClient db) : base(db) { } }
}
