using ScadaServer.Application.Interfaces;
using SqlSugar;
using ScadaServer.Domain.Entities;
using ScadaServer.Infrastructure.Persistence;
using ScadaServer.Infrastructure.DBEntities;

namespace ScadaServer.Infrastructure.Repositories
{
    public class HistoricalRecordRepository : IHistoricalRecordRepository
    {
        protected readonly ISqlSugarClient _db;

        public HistoricalRecordRepository(ISqlSugarClient db)
        {
            _db = db;
        }

        public virtual async Task<HistoricalRecord> GetByIdAsync(int id)
        {
            var dbEntity = await _db.Queryable<HistoricalRecordDbEntity>().In((long)id).FirstAsync();
            return dbEntity == null ? null : EntityMapper.MapToDomain(dbEntity);
        }

        public async Task<HistoricalRecord> GetByIdAsync(long id)
        {
            var dbEntity = await _db.Queryable<HistoricalRecordDbEntity>().In(id).FirstAsync();
            return dbEntity == null ? null : EntityMapper.MapToDomain(dbEntity);
        }

        public virtual async Task<List<HistoricalRecord>> GetListAsync()
        {
            var dbEntities = await _db.Queryable<HistoricalRecordDbEntity>().ToListAsync();
            return dbEntities.Select(EntityMapper.MapToDomain).ToList();
        }

        public virtual async Task<List<HistoricalRecord>> GetListAsync(System.Linq.Expressions.Expression<Func<HistoricalRecord, bool>> predicate)
        {
            var allEntities = await GetListAsync();
            return allEntities.AsQueryable().Where(predicate).ToList();
        }

        public virtual async Task InsertAsync(HistoricalRecord entity)
        {
            var dbEntity = EntityMapper.MapToDb(entity);
            var result = await _db.Insertable(dbEntity).ExecuteReturnEntityAsync();
            entity.Id = result.Id;
        }

        public virtual async Task UpdateAsync(HistoricalRecord entity)
        {
            var dbEntity = EntityMapper.MapToDb(entity);
            await _db.Updateable(dbEntity).ExecuteCommandAsync();
        }

        public virtual async Task DeleteAsync(HistoricalRecord entity)
        {
            var dbEntity = EntityMapper.MapToDb(entity);
            await _db.Deleteable(dbEntity).ExecuteCommandAsync();
        }

        public virtual async Task DeleteRangeAsync(System.Linq.Expressions.Expression<Func<HistoricalRecord, bool>> predicate)
        {
            var entities = await GetListAsync(predicate);
            foreach (var entity in entities)
            {
                await DeleteAsync(entity);
            }
        }

        public virtual async Task<bool> AnyAsync(System.Linq.Expressions.Expression<Func<HistoricalRecord, bool>> predicate)
        {
            var entities = await GetListAsync();
            return entities.AsQueryable().Any(predicate);
        }
    }
}