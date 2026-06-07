using ScadaServer.Application.Interfaces;
using SqlSugar;
using ScadaServer.Domain.Entities;
using ScadaServer.Infrastructure.Persistence;
using ScadaServer.Infrastructure.DBEntities;

namespace ScadaServer.Infrastructure.Repositories
{
    public class ScadaProjectRepository : IScadaProjectRepository
    {
        protected readonly ISqlSugarClient _db;

        public ScadaProjectRepository(ISqlSugarClient db)
        {
            _db = db;
        }

        public virtual async Task<ScadaProject> GetByIdAsync(int id)
        {
            var dbEntity = await _db.Queryable<ScadaProjectDbEntity>().In(id).FirstAsync();
            if (dbEntity == null) return null;

            var domainEntity = EntityMapper.MapToDomain(dbEntity);

            var pages = await _db.Queryable<ScadaPageDbEntity>()
                .Where(x => x.ProjectId == id)
                .ToListAsync();
            domainEntity.Pages = pages.Select(EntityMapper.MapToDomain).ToList();

            return domainEntity;
        }

        public virtual async Task<List<ScadaProject>> GetListAsync()
        {
            var dbEntities = await _db.Queryable<ScadaProjectDbEntity>().ToListAsync();
            var domainEntities = dbEntities.Select(EntityMapper.MapToDomain).ToList();

            foreach (var project in domainEntities)
            {
                var pages = await _db.Queryable<ScadaPageDbEntity>()
                    .Where(x => x.ProjectId == project.Id)
                    .ToListAsync();
                project.Pages = pages.Select(EntityMapper.MapToDomain).ToList();
            }

            return domainEntities;
        }

        public virtual async Task<List<ScadaProject>> GetListAsync(System.Linq.Expressions.Expression<Func<ScadaProject, bool>> predicate)
        {
            var allEntities = await GetListAsync();
            return allEntities.AsQueryable().Where(predicate).ToList();
        }

        public virtual async Task InsertAsync(ScadaProject entity)
        {
            var dbEntity = EntityMapper.MapToDb(entity);
            var result = await _db.Insertable(dbEntity).ExecuteReturnEntityAsync();
            entity.Id = result.Id;
        }

        public virtual async Task UpdateAsync(ScadaProject entity)
        {
            var dbEntity = EntityMapper.MapToDb(entity);
            await _db.Updateable(dbEntity).ExecuteCommandAsync();
        }

        public virtual async Task DeleteAsync(ScadaProject entity)
        {
            var dbEntity = EntityMapper.MapToDb(entity);
            await _db.Deleteable(dbEntity).ExecuteCommandAsync();
        }

        public virtual async Task DeleteRangeAsync(System.Linq.Expressions.Expression<Func<ScadaProject, bool>> predicate)
        {
            var entities = await GetListAsync(predicate);
            foreach (var entity in entities)
            {
                await DeleteAsync(entity);
            }
        }

        public virtual async Task<bool> AnyAsync(System.Linq.Expressions.Expression<Func<ScadaProject, bool>> predicate)
        {
            var entities = await GetListAsync();
            return entities.AsQueryable().Any(predicate);
        }
    }
}