using ScadaServer.Application.Interfaces;
using SqlSugar;
using ScadaServer.Domain.Entities;
using ScadaServer.Infrastructure.Persistence;
using ScadaServer.Infrastructure.DBEntities;

namespace ScadaServer.Infrastructure.Repositories
{
    public class DataModelRepository : IDataModelRepository
    {
        protected readonly ISqlSugarClient _db;

        public DataModelRepository(ISqlSugarClient db)
        {
            _db = db;
        }

        public async Task<DataModel> GetByIdAsync(int id)
        {
            var dbEntity = await _db.Queryable<DataModelDbEntity>()
                .In(id)
                .FirstAsync();

            if (dbEntity == null) return null;

            var domainEntity = EntityMapper.MapToDomain(dbEntity);

            var variables = await _db.Queryable<ModelVariableDbEntity>()
                .Where(x => x.ModelId == id)
                .ToListAsync();

            domainEntity.Variables = variables.Select(EntityMapper.MapToDomain).ToList();

            return domainEntity;
        }

        public async Task<List<DataModel>> GetListAsync()
        {
            var dbEntities = await _db.Queryable<DataModelDbEntity>().ToListAsync();
            var domainEntities = dbEntities.Select(EntityMapper.MapToDomain).ToList();

            foreach (var model in domainEntities)
            {
                var variables = await _db.Queryable<ModelVariableDbEntity>()
                    .Where(x => x.ModelId == model.Id)
                    .ToListAsync();
                model.Variables = variables.Select(EntityMapper.MapToDomain).ToList();
            }

            return domainEntities;
        }

        public async Task<List<DataModel>> GetListAsync(System.Linq.Expressions.Expression<Func<DataModel, bool>> predicate)
        {
            var allEntities = await GetListAsync();
            return allEntities.AsQueryable().Where(predicate).ToList();
        }

        public async Task InsertAsync(DataModel entity)
        {
            var dbEntity = EntityMapper.MapToDb(entity);
            var result = await _db.Insertable(dbEntity).ExecuteReturnEntityAsync();
            entity.Id = result.Id;

            if (entity.Variables != null)
            {
                foreach (var variable in entity.Variables)
                {
                    variable.ModelId = entity.Id;
                    await InsertVariableAsync(variable);
                }
            }
        }

        private async Task InsertVariableAsync(ModelVariable variable)
        {
            var dbEntity = EntityMapper.MapToDb(variable);
            var result = await _db.Insertable(dbEntity).ExecuteReturnEntityAsync();
            variable.Id = result.Id;
        }

        public async Task UpdateAsync(DataModel entity)
        {
            var dbEntity = EntityMapper.MapToDb(entity);
            await _db.Updateable(dbEntity).ExecuteCommandAsync();
        }

        public async Task DeleteAsync(DataModel entity)
        {
            var dbEntity = EntityMapper.MapToDb(entity);
            await _db.Deleteable(dbEntity).ExecuteCommandAsync();
        }

        public async Task DeleteRangeAsync(System.Linq.Expressions.Expression<Func<DataModel, bool>> predicate)
        {
            var entities = await GetListAsync(predicate);
            foreach (var entity in entities)
            {
                await DeleteAsync(entity);
            }
        }

        public async Task<bool> AnyAsync(System.Linq.Expressions.Expression<Func<DataModel, bool>> predicate)
        {
            var entities = await GetListAsync();
            return entities.AsQueryable().Any(predicate);
        }
    }
}