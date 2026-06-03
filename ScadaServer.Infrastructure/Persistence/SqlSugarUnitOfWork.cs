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

        public async Task<ITransactionScope> BeginTransactionAsync()
        {
            _db.AsTenant().BeginTran();
            return new TransactionScope(_db);
        }

        public void Dispose()
        {
            // Managed by DI Scoped lifecycle
        }

        private class TransactionScope : ITransactionScope
        {
            private readonly ISqlSugarClient _db;
            private bool _isCommitted = false;

            public TransactionScope(ISqlSugarClient db)
            {
                _db = db;
            }

            public async Task CommitAsync()
            {
                await _db.AsTenant().CommitTranAsync();
                _isCommitted = true;
            }

            public async Task RollbackAsync()
            {
                await _db.AsTenant().RollbackTranAsync();
                _isCommitted = true;
            }

            public async ValueTask DisposeAsync()
            {
                if (!_isCommitted)
                {
                    try
                    {
                        await _db.AsTenant().RollbackTranAsync();
                    }
                    catch
                    {
                        // 忽略回滚异常，避免掩盖原始异常
                    }
                }
            }
        }
    }
}
