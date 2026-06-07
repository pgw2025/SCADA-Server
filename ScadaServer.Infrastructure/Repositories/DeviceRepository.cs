using ScadaServer.Application.Interfaces;
using SqlSugar;
using ScadaServer.Domain.Entities;
using ScadaServer.Infrastructure.DBEntities;
using ScadaServer.Infrastructure.Persistence;

namespace ScadaServer.Infrastructure.Repositories
{
    public class DeviceRepository : IDeviceRepository
    {
        protected readonly ISqlSugarClient _db;

        public DeviceRepository(ISqlSugarClient db)
        {
            _db = db;
        }

        public virtual async Task<Device> GetByIdAsync(int id)
        {
            var dbEntity = await _db.Queryable<DeviceDbEntity>()
                .In(id)
                .FirstAsync();
            return dbEntity == null ? null : EntityMapper.MapToDomain(dbEntity);
        }

        public virtual async Task<List<Device>> GetListAsync()
        {
            var dbEntities = await _db.Queryable<DeviceDbEntity>().ToListAsync();
            return dbEntities.Select(EntityMapper.MapToDomain).ToList();
        }

        public virtual async Task<List<Device>> GetListAsync(System.Linq.Expressions.Expression<Func<Device, bool>> predicate)
        {
            var allEntities = await GetListAsync();
            return allEntities.AsQueryable().Where(predicate).ToList();
        }

        public virtual async Task InsertAsync(Device entity)
        {
            var dbEntity = EntityMapper.MapToDb(entity);
            var result = await _db.Insertable(dbEntity).ExecuteReturnEntityAsync();
            entity.Id = result.Id;
        }

        public virtual async Task UpdateAsync(Device entity)
        {
            var dbEntity = EntityMapper.MapToDb(entity);
            await _db.Updateable(dbEntity).ExecuteCommandAsync();
        }

        public virtual async Task DeleteAsync(Device entity)
        {
            var dbEntity = EntityMapper.MapToDb(entity);
            await _db.Deleteable(dbEntity).ExecuteCommandAsync();
        }

        public virtual async Task DeleteRangeAsync(System.Linq.Expressions.Expression<Func<Device, bool>> predicate)
        {
            var entities = await GetListAsync(predicate);
            foreach (var entity in entities)
            {
                await DeleteAsync(entity);
            }
        }

        public virtual async Task<bool> AnyAsync(System.Linq.Expressions.Expression<Func<Device, bool>> predicate)
        {
            var entities = await GetListAsync();
            return entities.AsQueryable().Any(predicate);
        }

        public async Task<List<Device>> GetActiveDevicesAsync()
        {
            var dbEntities = await _db.Queryable<DeviceDbEntity>()
                .Where(d => d.IsEnabled)
                .ToListAsync();
            return dbEntities.Select(EntityMapper.MapToDomain).ToList();
        }

        public async Task<Device?> GetByIdWithConfigAsync(int id)
        {
            var dbEntity = await _db.Queryable<DeviceDbEntity>()
                .Includes(d => d.Config)
                .Includes(d => d.Area)
                .Includes(d => d.Model)
                .FirstAsync(d => d.Id == id);
            return dbEntity == null ? null : EntityMapper.MapToDomain(dbEntity);
        }

        public async Task<List<Device>> GetListWithConfigAsync()
        {
            var dbEntities = await _db.Queryable<DeviceDbEntity>()
                .Includes(d => d.Config)
                .Includes(d => d.Area)
                .Includes(d => d.Model)
                .ToListAsync();
            return dbEntities.Select(EntityMapper.MapToDomain).ToList();
        }

        public async Task<Device?> GetByKeyAsync(string key)
        {
            var dbEntity = await _db.Queryable<DeviceDbEntity>()
                .Includes(d => d.Config)
                .FirstAsync(d => d.Key == key);
            return dbEntity == null ? null : EntityMapper.MapToDomain(dbEntity);
        }
    }
}