using ScadaServer.Application.Interfaces;
using SqlSugar;
using ScadaServer.Domain.Entities;
using ScadaServer.Infrastructure.Persistence;

namespace ScadaServer.Infrastructure.Repositories
{
    public class DeviceRepository : SqlSugarRepository<Device>, IDeviceRepository
    {
        public DeviceRepository(ISqlSugarClient db) : base(db) { }

        public async Task<List<Device>> GetActiveDevicesAsync()
        {
            return await _db.Queryable<Device>()
                .Where(d => d.Status == ScadaServer.Domain.Enums.DeviceStatus.Online)
                .ToListAsync();
        }
    }
}
