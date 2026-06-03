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
            // 使用 SqlSugar 原生的异步提交
            await _db.AsTenant().CommitTranAsync();
        }

        public async Task RollbackTranAsync()
        {
            // 使用 SqlSugar 原生的异步回滚
            await _db.AsTenant().RollbackTranAsync();
        }

        public async Task<ITransactionScope> BeginTransactionAsync()
        {
            // 使用异步启动事务
            await _db.AsTenant().BeginTranAsync();
            return new TransactionScope(_db);
        }

        public void Dispose()
        {
            // 由 DI 容器管理生命周期，此处无需手动释放
        }

        private class TransactionScope : ITransactionScope
        {
            private readonly ISqlSugarClient _db;
            private bool _isCompleted = false; // 变更为更准确的命名

            public TransactionScope(ISqlSugarClient db)
            {
                _db = db;
            }

            public async Task CommitAsync()
            {
                await _db.AsTenant().CommitTranAsync();
                _isCompleted = true;
            }

            public async Task RollbackAsync()
            {
                await _db.AsTenant().RollbackTranAsync();
                _isCompleted = true;
            }

            public async ValueTask DisposeAsync()
            {
                if (!_isCompleted)
                {
                    try
                    {
                        // 若未显式提交或回滚，则在释放时自动回滚
                        await _db.AsTenant().RollbackTranAsync();
                    }
                    catch (Exception ex)
                    {
                        // 记录日志，避免在 Dispose 中抛出异常影响后续逻辑
                        System.Diagnostics.Debug.WriteLine($"事务自动回滚失败：{ex.Message}");
                    }
                    finally
                    {
                        _isCompleted = true;
                    }
                }
            }
        }
    }
}