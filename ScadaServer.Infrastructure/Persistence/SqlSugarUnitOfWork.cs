using ScadaServer.Application.Interfaces;
using SqlSugar;

namespace ScadaServer.Infrastructure.Persistence
{
    public class SqlSugarUnitOfWork : IUnitOfWork
    {
        private readonly ISqlSugarClient _db;

        public SqlSugarUnitOfWork(ISqlSugarClient db)
        {
            _db = db;
        }

        public void BeginTran()
        {
            _db.AsTenant().BeginTran();
        }

        public async Task CommitTranAsync()
        {
            try
            {
                await Task.Run(() => _db.AsTenant().CommitTran());
            }
            catch
            {
                await RollbackTranAsync();
                throw;
            }
        }

        public async Task RollbackTranAsync()
        {
            await Task.Run(() => _db.AsTenant().RollbackTran());
        }

        public void Dispose()
        {
            // Managed by DI Scoped lifecycle
        }
    }
}
