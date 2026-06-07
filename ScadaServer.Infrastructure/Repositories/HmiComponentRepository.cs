using ScadaServer.Application.Interfaces;
using SqlSugar;
using ScadaServer.Domain.Entities;
using ScadaServer.Infrastructure.Persistence;
using ScadaServer.Infrastructure.DBEntities;

namespace ScadaServer.Infrastructure.Repositories
{
    public class HmiComponentRepository : IHmiComponentRepository
    {
        protected readonly ISqlSugarClient _db;

        public HmiComponentRepository(ISqlSugarClient db)
        {
            _db = db;
        }

        public virtual async Task<HmiComponent> GetByIdAsync(int id)
        {
            var dbEntity = await _db.Queryable<HmiComponentDbEntity>().In(id).FirstAsync();
            return dbEntity == null ? null : EntityMapper.MapToDomain(dbEntity);
        }

        public virtual async Task<List<HmiComponent>> GetListAsync()
        {
            var dbEntities = await _db.Queryable<HmiComponentDbEntity>().ToListAsync();
            return dbEntities.Select(EntityMapper.MapToDomain).ToList();
        }

        public virtual async Task<List<HmiComponent>> GetListAsync(System.Linq.Expressions.Expression<Func<HmiComponent, bool>> predicate)
        {
            var allEntities = await GetListAsync();
            return allEntities.AsQueryable().Where(predicate).ToList();
        }

        public virtual async Task InsertAsync(HmiComponent entity)
        {
            var dbEntity = EntityMapper.MapToDb(entity);
            var result = await _db.Insertable(dbEntity).ExecuteReturnEntityAsync();
            entity.Id = result.Id;
        }

        public virtual async Task UpdateAsync(HmiComponent entity)
        {
            var dbEntity = EntityMapper.MapToDb(entity);
            await _db.Updateable(dbEntity).ExecuteCommandAsync();
        }

        public virtual async Task DeleteAsync(HmiComponent entity)
        {
            var dbEntity = EntityMapper.MapToDb(entity);
            await _db.Deleteable(dbEntity).ExecuteCommandAsync();
        }

        public virtual async Task DeleteRangeAsync(System.Linq.Expressions.Expression<Func<HmiComponent, bool>> predicate)
        {
            var entities = await GetListAsync(predicate);
            foreach (var entity in entities)
            {
                await DeleteAsync(entity);
            }
        }

        public virtual async Task<bool> AnyAsync(System.Linq.Expressions.Expression<Func<HmiComponent, bool>> predicate)
        {
            var entities = await GetListAsync();
            return entities.AsQueryable().Any(predicate);
        }
    }
}