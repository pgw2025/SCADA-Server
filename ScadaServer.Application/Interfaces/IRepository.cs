using ScadaServer.Domain.Entities;
using ScadaServer.Application.DTOs;
using System.Linq.Expressions;

namespace ScadaServer.Application.Interfaces
{
    public interface IRepository<T> where T : class, new()
    {
        Task<T> GetByIdAsync(dynamic id);
        Task<List<T>> GetListAsync();
        Task<List<T>> GetListAsync(Expression<Func<T, bool>> predicate);
        Task InsertAsync(T entity);
        Task UpdateAsync(T entity);
        Task DeleteAsync(T entity);
        Task DeleteRangeAsync(Expression<Func<T, bool>> predicate);
        Task<bool> AnyAsync(Expression<Func<T, bool>> predicate);
    }
}

