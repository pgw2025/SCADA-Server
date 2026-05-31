using ScadaServer.Application.Interfaces;
using ScadaServer.Domain.Entities;
using SqlSugar;

namespace ScadaServer.Infrastructure.Persistence
{
    public class SqlSugarDeviceRepository : SqlSugarRepository<DeviceEntity>, IDeviceRepository
    {
        public SqlSugarDeviceRepository(ISqlSugarClient db) : base(db) { }

        public async Task<List<DeviceEntity>> GetActiveDevicesWithPointsAsync()
        {
            return await Db.Queryable<DeviceEntity>()
                .Where(d => d.Status == "Online")
                .ToListAsync();
        }
    }
}
