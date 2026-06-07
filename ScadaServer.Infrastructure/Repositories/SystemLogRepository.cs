using ScadaServer.Application.Interfaces;
using SqlSugar;
using ScadaServer.Domain.Entities;
using ScadaServer.Infrastructure.Persistence;
using ScadaServer.Infrastructure.DBEntities;

namespace ScadaServer.Infrastructure.Repositories
{
    public class SystemLogRepository : ISystemLogRepository
    {
        protected readonly ISqlSugarClient _db;

        public SystemLogRepository(ISqlSugarClient db)
        {
            _db = db;
        }

        public virtual async Task<SystemLog> GetByIdAsync(int id)
        {
            var dbEntity = await _db.Queryable<SystemLogDbEntity>().In(id).FirstAsync();
            return dbEntity == null ? null : EntityMapper.MapToDomain(dbEntity);
        }

        public virtual async Task<List<SystemLog>> GetListAsync()
        {
            var dbEntities = await _db.Queryable<SystemLogDbEntity>().ToListAsync();
            return dbEntities.Select(EntityMapper.MapToDomain).ToList();
        }

        public virtual async Task<List<SystemLog>> GetListAsync(System.Linq.Expressions.Expression<Func<SystemLog, bool>> predicate)
        {
            var allEntities = await GetListAsync();
            return allEntities.AsQueryable().Where(predicate).ToList();
        }

        public virtual async Task InsertAsync(SystemLog entity)
        {
            var dbEntity = EntityMapper.MapToDb(entity);
            var result = await _db.Insertable(dbEntity).ExecuteReturnEntityAsync();
            entity.Id = result.Id;
        }

        public virtual async Task UpdateAsync(SystemLog entity)
        {
            var dbEntity = EntityMapper.MapToDb(entity);
            await _db.Updateable(dbEntity).ExecuteCommandAsync();
        }

        public virtual async Task DeleteAsync(SystemLog entity)
        {
            var dbEntity = EntityMapper.MapToDb(entity);
            await _db.Deleteable(dbEntity).ExecuteCommandAsync();
        }

        public virtual async Task DeleteRangeAsync(System.Linq.Expressions.Expression<Func<SystemLog, bool>> predicate)
        {
            var entities = await GetListAsync(predicate);
            foreach (var entity in entities)
            {
                await DeleteAsync(entity);
            }
        }

        public virtual async Task<bool> AnyAsync(System.Linq.Expressions.Expression<Func<SystemLog, bool>> predicate)
        {
            var entities = await GetListAsync();
            return entities.AsQueryable().Any(predicate);
        }
    }
}