using SqlSugar;
using ScadaServer.Application.Interfaces;
using System.Linq.Expressions;

namespace ScadaServer.Infrastructure.Persistence
{
    public class SqlSugarRepository<T> : IRepository<T> where T : class, new()
    {
        protected readonly ISqlSugarClient _db;

        public SqlSugarRepository(ISqlSugarClient db)
        {
            _db = db;
        }

        public virtual async Task<T> GetByIdAsync(dynamic id)
        {
            return await _db.Queryable<T>().In(id).FirstAsync();
        }

        public virtual async Task<List<T>> GetListAsync()
        {
            return await _db.Queryable<T>().ToListAsync();
        }

        public virtual async Task<List<T>> GetListAsync(Expression<Func<T, bool>> predicate)
        {
            return await _db.Queryable<T>().Where(predicate).ToListAsync();
        }

        public virtual async Task InsertAsync(T entity)
        {
            await _db.Insertable(entity).ExecuteCommandAsync();
        }

        public virtual async Task UpdateAsync(T entity)
        {
            await _db.Updateable(entity).ExecuteCommandAsync();
        }

        public virtual async Task DeleteAsync(T entity)
        {
            await _db.Deleteable(entity).ExecuteCommandAsync();
        }

        public virtual async Task DeleteRangeAsync(Expression<Func<T, bool>> predicate)
        {
            await _db.Deleteable<T>().Where(predicate).ExecuteCommandAsync();
        }
    }
}
