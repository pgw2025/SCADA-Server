using SqlSugar;
using ScadaServer.Domain.Entities;
using ScadaServer.Infrastructure.Persistence;

namespace ScadaServer.Infrastructure.Repositories
{
    public class SensorRepository : SqlSugarRepository<Sensor> { public SensorRepository(ISqlSugarClient db) : base(db) { } }
}
