using ScadaServer.Domain.Entities;
using ScadaServer.Application.DTOs;
namespace ScadaServer.Application.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        void BeginTran();
        Task CommitTranAsync();
        Task RollbackTranAsync();
    }
}

