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
            await _db.AsTenant().CommitTranAsync();
        }

        public async Task RollbackTranAsync()
        {
            await _db.AsTenant().RollbackTranAsync();
        }

        public void Dispose()
        {
            // Managed by DI Scoped lifecycle
        }
    }
}
