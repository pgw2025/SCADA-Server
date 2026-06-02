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
            // 使用 ExecuteReturnEntityAsync 会自动将数据库生成的自增 ID 回填到 entity 对象中
            var result = await _db.Insertable(entity).ExecuteReturnEntityAsync();
            
            // 为了确保调用方拿到的就是同一个引用且 ID 已更新，我们也可以手动处理（如果 SqlSugar 版本有差异）
            // 但在标准 SqlSugar 中，ExecuteReturnEntityAsync 已经足够。
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
}
