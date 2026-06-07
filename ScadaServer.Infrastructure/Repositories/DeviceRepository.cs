using ScadaServer.Application.Interfaces;
using SqlSugar;
using ScadaServer.Domain.Entities;
using ScadaServer.Domain.Enums;
using ScadaServer.Infrastructure.DBEntities;
using ScadaServer.Infrastructure.Persistence;

namespace ScadaServer.Infrastructure.Repositories
{
    public class DeviceRepository : SqlSugarRepository<Device>, IDeviceRepository
    {
        public DeviceRepository(ISqlSugarClient db) : base(db) { }

        public async Task<List<Device>> GetActiveDevicesAsync()
        {
            var dbEntities = await _db.Queryable<DeviceDbEntity>()
                .Where(d => d.IsEnabled)
                .ToListAsync();
            return dbEntities.Select(MapToDomain).ToList();
        }

        public async Task<Device?> GetByIdWithConfigAsync(int id)
        {
            var dbEntity = await _db.Queryable<DeviceDbEntity>()
                .Includes(d => d.Config)
                .Includes(d => d.Area)
                .Includes(d => d.Model)
                .FirstAsync(d => d.Id == id);
            return dbEntity == null ? null : MapToDomain(dbEntity);
        }

        public async Task<List<Device>> GetListWithConfigAsync()
        {
            var dbEntities = await _db.Queryable<DeviceDbEntity>()
                .Includes(d => d.Config)
                .Includes(d => d.Area)
                .Includes(d => d.Model)
                .ToListAsync();
            return dbEntities.Select(MapToDomain).ToList();
        }

        public async Task<Device?> GetByKeyAsync(string key)
        {
            var dbEntity = await _db.Queryable<DeviceDbEntity>()
                .Includes(d => d.Config)
                .FirstAsync(d => d.Key == key);
            return dbEntity == null ? null : MapToDomain(dbEntity);
        }

        private static Device MapToDomain(DeviceDbEntity dbEntity)
        {
            return new Device
            {
                Id = dbEntity.Id,
                Name = dbEntity.Name,
                Key = dbEntity.Key,
                AreaId = dbEntity.AreaId,
                Area = dbEntity.Area == null ? null : new Area
                {
                    Id = dbEntity.Area.Id,
                    Name = dbEntity.Area.Name,
                    Description = dbEntity.Area.Description
                },
                ModelId = dbEntity.ModelId,
                Model = dbEntity.Model == null ? null : new DataModel
                {
                    Id = dbEntity.Model.Id,
                    Name = dbEntity.Model.Name,
                    Description = dbEntity.Model.Description,
                    Type = dbEntity.Model.Type,
                    CreatedAt = dbEntity.Model.CreatedAt,
                    UpdatedAt = dbEntity.Model.UpdatedAt
                },
                Type = dbEntity.Type,
                IsEnabled = dbEntity.IsEnabled,
                PollingInterval = dbEntity.PollingInterval,
                DriverName = dbEntity.DriverName,
                CreatedAt = dbEntity.CreatedAt,
                UpdatedAt = dbEntity.UpdatedAt,
                LastCommunicationTime = dbEntity.LastCommunicationTime,
                Config = dbEntity.Config == null ? null : new DeviceConfig
                {
                    DeviceId = dbEntity.Config.DeviceId,
                    JsonConfig = dbEntity.Config.JsonConfig,
                    Version = dbEntity.Config.Version,
                    UpdatedAt = dbEntity.Config.UpdatedAt
                }
            };
        }
    }
}