using SqlSugar;
using ScadaServer.Domain.Entities;
using ScadaServer.Infrastructure.Persistence;

namespace ScadaServer.Infrastructure.Repositories
{
    public class DeviceRepository : SqlSugarRepository<Device> { public DeviceRepository(ISqlSugarClient db) : base(db) { } }
}
