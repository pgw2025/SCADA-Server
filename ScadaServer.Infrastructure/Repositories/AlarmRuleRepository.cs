using ScadaServer.Application.Interfaces;
using SqlSugar;
using ScadaServer.Domain.Entities;
using ScadaServer.Infrastructure.Persistence;
using ScadaServer.Infrastructure.DBEntities;

namespace ScadaServer.Infrastructure.Repositories
{
    public class AlarmRuleRepository : IAlarmRuleRepository
    {
        protected readonly ISqlSugarClient _db;

        public AlarmRuleRepository(ISqlSugarClient db)
        {
            _db = db;
        }

        public virtual async Task<AlarmRule> GetByIdAsync(int id)
        {
            var dbEntity = await _db.Queryable<AlarmRuleDbEntity>().In(id).FirstAsync();
            return dbEntity == null ? null : EntityMapper.MapToDomain(dbEntity);
        }

        public virtual async Task<List<AlarmRule>> GetListAsync()
        {
            var dbEntities = await _db.Queryable<AlarmRuleDbEntity>().ToListAsync();
            return dbEntities.Select(EntityMapper.MapToDomain).ToList();
        }

        public virtual async Task<List<AlarmRule>> GetListAsync(System.Linq.Expressions.Expression<Func<AlarmRule, bool>> predicate)
        {
            var allEntities = await GetListAsync();
            return allEntities.AsQueryable().Where(predicate).ToList();
        }

        public virtual async Task InsertAsync(AlarmRule entity)
        {
            var dbEntity = EntityMapper.MapToDb(entity);
            var result = await _db.Insertable(dbEntity).ExecuteReturnEntityAsync();
            entity.Id = result.Id;
        }

        public virtual async Task UpdateAsync(AlarmRule entity)
        {
            var dbEntity = EntityMapper.MapToDb(entity);
            await _db.Updateable(dbEntity).ExecuteCommandAsync();
        }

        public virtual async Task DeleteAsync(AlarmRule entity)
        {
            var dbEntity = EntityMapper.MapToDb(entity);
            await _db.Deleteable(dbEntity).ExecuteCommandAsync();
        }

        public virtual async Task DeleteRangeAsync(System.Linq.Expressions.Expression<Func<AlarmRule, bool>> predicate)
        {
            var entities = await GetListAsync(predicate);
            foreach (var entity in entities)
            {
                await DeleteAsync(entity);
            }
        }

        public virtual async Task<bool> AnyAsync(System.Linq.Expressions.Expression<Func<AlarmRule, bool>> predicate)
        {
            var entities = await GetListAsync();
            return entities.AsQueryable().Any(predicate);
        }
    }
}