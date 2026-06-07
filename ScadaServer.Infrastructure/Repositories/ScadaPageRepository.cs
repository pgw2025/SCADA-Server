using ScadaServer.Application.Interfaces;
using SqlSugar;
using ScadaServer.Domain.Entities;
using ScadaServer.Infrastructure.Persistence;
using ScadaServer.Infrastructure.DBEntities;

namespace ScadaServer.Infrastructure.Repositories
{
    public class ScadaPageRepository : IScadaPageRepository
    {
        protected readonly ISqlSugarClient _db;

        public ScadaPageRepository(ISqlSugarClient db)
        {
            _db = db;
        }

        public virtual async Task<ScadaPage> GetByIdAsync(int id)
        {
            var dbEntity = await _db.Queryable<ScadaPageDbEntity>().In(id).FirstAsync();
            if (dbEntity == null) return null;

            var domainEntity = EntityMapper.MapToDomain(dbEntity);

            var components = await _db.Queryable<HmiComponentDbEntity>()
                .Where(x => x.PageId == id)
                .ToListAsync();
            domainEntity.Components = components.Select(EntityMapper.MapToDomain).ToList();

            return domainEntity;
        }

        public virtual async Task<List<ScadaPage>> GetListAsync()
        {
            var dbEntities = await _db.Queryable<ScadaPageDbEntity>().ToListAsync();
            var domainEntities = dbEntities.Select(EntityMapper.MapToDomain).ToList();

            foreach (var page in domainEntities)
            {
                var components = await _db.Queryable<HmiComponentDbEntity>()
                    .Where(x => x.PageId == page.Id)
                    .ToListAsync();
                page.Components = components.Select(EntityMapper.MapToDomain).ToList();
            }

            return domainEntities;
        }

        public virtual async Task<List<ScadaPage>> GetListAsync(System.Linq.Expressions.Expression<Func<ScadaPage, bool>> predicate)
        {
            var allEntities = await GetListAsync();
            return allEntities.AsQueryable().Where(predicate).ToList();
        }

        public virtual async Task InsertAsync(ScadaPage entity)
        {
            var dbEntity = EntityMapper.MapToDb(entity);
            var result = await _db.Insertable(dbEntity).ExecuteReturnEntityAsync();
            entity.Id = result.Id;
        }

        public virtual async Task UpdateAsync(ScadaPage entity)
        {
            var dbEntity = EntityMapper.MapToDb(entity);
            await _db.Updateable(dbEntity).ExecuteCommandAsync();
        }

        public virtual async Task DeleteAsync(ScadaPage entity)
        {
            var dbEntity = EntityMapper.MapToDb(entity);
            await _db.Deleteable(dbEntity).ExecuteCommandAsync();
        }

        public virtual async Task DeleteRangeAsync(System.Linq.Expressions.Expression<Func<ScadaPage, bool>> predicate)
        {
            var entities = await GetListAsync(predicate);
            foreach (var entity in entities)
            {
                await DeleteAsync(entity);
            }
        }

        public virtual async Task<bool> AnyAsync(System.Linq.Expressions.Expression<Func<ScadaPage, bool>> predicate)
        {
            var entities = await GetListAsync();
            return entities.AsQueryable().Any(predicate);
        }
    }
}