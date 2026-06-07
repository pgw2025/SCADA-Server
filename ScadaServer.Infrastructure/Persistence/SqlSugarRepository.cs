using SqlSugar;
using ScadaServer.Domain.Interfaces;
using System.Linq.Expressions;

namespace ScadaServer.Infrastructure.Persistence
{
    public class SqlSugarRepository<T, TKey> : IRepository<T, TKey> where T : class, new()
    {
        protected readonly ISqlSugarClient _db;

        public SqlSugarRepository(ISqlSugarClient db)
        {
            _db = db;
        }

        public virtual async Task<T> GetByIdAsync(TKey id)
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
            await _db.Insertable(entity).ExecuteReturnEntityAsync();
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

        public virtual async Task<bool> AnyAsync(Expression<Func<T, bool>> predicate)
        {
            return await _db.Queryable<T>().AnyAsync(predicate);
        }
    }

    public class SqlSugarRepository<T> : SqlSugarRepository<T, int>, IRepository<T> where T : class, new()
    {
        public SqlSugarRepository(ISqlSugarClient db) : base(db)
        {
        }
    }
}
