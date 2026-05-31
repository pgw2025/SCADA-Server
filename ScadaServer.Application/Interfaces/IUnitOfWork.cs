namespace ScadaServer.Application.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        void BeginTran();
        Task CommitTranAsync();
        Task RollbackTranAsync();
    }
}
