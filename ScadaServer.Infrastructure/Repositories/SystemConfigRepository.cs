using ScadaServer.Application.Interfaces;
using SqlSugar;
using ScadaServer.Domain.Entities;
using ScadaServer.Infrastructure.Persistence;
using ScadaServer.Infrastructure.DBEntities;

namespace ScadaServer.Infrastructure.Repositories
{
    public class SystemConfigRepository : ISystemConfigRepository
    {
        protected readonly ISqlSugarClient _db;

        public SystemConfigRepository(ISqlSugarClient db)
        {
            _db = db;
        }

        public virtual async Task<SystemConfig> GetByIdAsync(int id)
        {
            var dbEntity = await _db.Queryable<SystemConfigDbEntity>().In(id).FirstAsync();
            return dbEntity == null ? null : EntityMapper.MapToDomain(dbEntity);
        }

        public virtual async Task<List<SystemConfig>> GetListAsync()
        {
            var dbEntities = await _db.Queryable<SystemConfigDbEntity>().ToListAsync();
            return dbEntities.Select(EntityMapper.MapToDomain).ToList();
        }

        public virtual async Task<List<SystemConfig>> GetListAsync(System.Linq.Expressions.Expression<Func<SystemConfig, bool>> predicate)
        {
            var allEntities = await GetListAsync();
            return allEntities.AsQueryable().Where(predicate).ToList();
        }

        public virtual async Task InsertAsync(SystemConfig entity)
        {
            var dbEntity = EntityMapper.MapToDb(entity);
            var result = await _db.Insertable(dbEntity).ExecuteReturnEntityAsync();
            entity.Id = result.Id;
        }

        public virtual async Task UpdateAsync(SystemConfig entity)
        {
            var dbEntity = EntityMapper.MapToDb(entity);
            await _db.Updateable(dbEntity).ExecuteCommandAsync();
        }

        public virtual async Task DeleteAsync(SystemConfig entity)
        {
            var dbEntity = EntityMapper.MapToDb(entity);
            await _db.Deleteable(dbEntity).ExecuteCommandAsync();
        }

        public virtual async Task DeleteRangeAsync(System.Linq.Expressions.Expression<Func<SystemConfig, bool>> predicate)
        {
            var entities = await GetListAsync(predicate);
            foreach (var entity in entities)
            {
                await DeleteAsync(entity);
            }
        }

        public virtual async Task<bool> AnyAsync(System.Linq.Expressions.Expression<Func<SystemConfig, bool>> predicate)
        {
            var entities = await GetListAsync();
            return entities.AsQueryable().Any(predicate);
        }
    }
}