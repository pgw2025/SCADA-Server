using ScadaServer.Application.Interfaces;
using SqlSugar;
using ScadaServer.Domain.Entities;
using ScadaServer.Domain.Enums;
using ScadaServer.Infrastructure.Persistence;

namespace ScadaServer.Infrastructure.Repositories
{
    public class DeviceRepository : SqlSugarRepository<Device>, IDeviceRepository
    {
        public DeviceRepository(ISqlSugarClient db) : base(db) { }

        public async Task<List<Device>> GetActiveDevicesAsync()
        {
            return await _db.Queryable<Device>()
                .Where(d => d.IsEnabled)
                .ToListAsync();
        }

        public async Task<Device?> GetByIdWithConfigAsync(int id)
        {
            return await _db.Queryable<Device>()
                .Includes(d => d.Config)
                .Includes(d => d.Area)
                .Includes(d => d.Model)
                .FirstAsync(d => d.Id == id);
        }

        public async Task<List<Device>> GetListWithConfigAsync()
        {
            return await _db.Queryable<Device>()
                .Includes(d => d.Config)
                .Includes(d => d.Area)
                .Includes(d => d.Model)
                .ToListAsync();
        }

        public async Task<Device?> GetByKeyAsync(string key)
        {
            return await _db.Queryable<Device>()
                .Includes(d => d.Config)
                .FirstAsync(d => d.Key == key);
        }
    }
}
