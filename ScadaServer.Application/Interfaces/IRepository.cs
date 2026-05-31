using System.Linq.Expressions;

namespace ScadaServer.Application.Interfaces
{
    public interface IRepository<T> where T : class, new()
    {
        Task<T> GetByIdAsync(dynamic id);
        Task<List<T>> GetListAsync();
        Task InsertAsync(T entity);
        Task UpdateAsync(T entity);
        Task DeleteAsync(T entity);
        Task DeleteRangeAsync(Expression<Func<T, bool>> predicate);
    }
}
