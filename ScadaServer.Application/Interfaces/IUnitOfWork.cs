using ScadaServer.Domain.Entities;
using ScadaServer.Application.DTOs;
namespace ScadaServer.Application.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        void BeginTran();
        Task CommitTranAsync();
        Task RollbackTranAsync();
        Task<ITransactionScope> BeginTransactionAsync();
    }

    public interface ITransactionScope : IAsyncDisposable
    {
        Task CommitAsync();
        Task RollbackAsync();
    }
}

