using ScadaServer.Application.Interfaces;
using SqlSugar;
using ScadaServer.Domain.Entities;
using ScadaServer.Infrastructure.Persistence;
using ScadaServer.Infrastructure.DBEntities;

namespace ScadaServer.Infrastructure.Repositories
{
    public class RealtimeDataRepository : IRealtimeDataRepository
    {
        protected readonly ISqlSugarClient _db;

        public RealtimeDataRepository(ISqlSugarClient db)
        {
            _db = db;
        }

        public virtual async Task<RealtimeData> GetByIdAsync(int id)
        {
            return null;
        }

        public async Task<RealtimeData> GetByIdAsync(int deviceId, string variableKey)
        {
            var dbEntity = await _db.Queryable<RealtimeDataDbEntity>()
                .Where(x => x.DeviceId == deviceId && x.VariableKey == variableKey)
                .FirstAsync();
            return dbEntity == null ? null : EntityMapper.MapToDomain(dbEntity);
        }

        public virtual async Task<List<RealtimeData>> GetListAsync()
        {
            var dbEntities = await _db.Queryable<RealtimeDataDbEntity>().ToListAsync();
            return dbEntities.Select(EntityMapper.MapToDomain).ToList();
        }

        public virtual async Task<List<RealtimeData>> GetListAsync(System.Linq.Expressions.Expression<Func<RealtimeData, bool>> predicate)
        {
            var allEntities = await GetListAsync();
            return allEntities.AsQueryable().Where(predicate).ToList();
        }

        public virtual async Task InsertAsync(RealtimeData entity)
        {
            var dbEntity = EntityMapper.MapToDb(entity);
            await _db.Insertable(dbEntity).ExecuteCommandAsync();
        }

        public virtual async Task UpdateAsync(RealtimeData entity)
        {
            var dbEntity = EntityMapper.MapToDb(entity);
            await _db.Updateable(dbEntity).ExecuteCommandAsync();
        }

        public virtual async Task DeleteAsync(RealtimeData entity)
        {
            var dbEntity = EntityMapper.MapToDb(entity);
            await _db.Deleteable(dbEntity).ExecuteCommandAsync();
        }

        public virtual async Task DeleteRangeAsync(System.Linq.Expressions.Expression<Func<RealtimeData, bool>> predicate)
        {
            var entities = await GetListAsync(predicate);
            foreach (var entity in entities)
            {
                await DeleteAsync(entity);
            }
        }

        public virtual async Task<bool> AnyAsync(System.Linq.Expressions.Expression<Func<RealtimeData, bool>> predicate)
        {
            var entities = await GetListAsync();
            return entities.AsQueryable().Any(predicate);
        }
    }
}