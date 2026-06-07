using System.Linq.Expressions;

namespace ScadaServer.Domain.Interfaces
{
    /// <summary>
    /// Base repository interface for entities with a specific key type.
    /// </summary>
    public interface IRepository<T, TKey> where T : class, new()
    {
        Task<T> GetByIdAsync(TKey id);
        Task<List<T>> GetListAsync();
        Task<List<T>> GetListAsync(Expression<Func<T, bool>> predicate);
        Task InsertAsync(T entity);
        Task UpdateAsync(T entity);
        Task DeleteAsync(T entity);
        Task DeleteRangeAsync(Expression<Func<T, bool>> predicate);
        Task<bool> AnyAsync(Expression<Func<T, bool>> predicate);
    }
}
