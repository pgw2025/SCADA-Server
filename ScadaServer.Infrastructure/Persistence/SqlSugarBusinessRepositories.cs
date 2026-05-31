using SqlSugar;
using ScadaServer.Application.Interfaces;
using ScadaServer.Domain.Entities;

namespace ScadaServer.Infrastructure.Persistence
{
    public class SqlSugarAssetRepository : IAssetRepository
    {
        private readonly ISqlSugarClient _db;

        public SqlSugarAssetRepository(ISqlSugarClient db)
        {
            _db = db;
        }

        public async Task<List<Area>> GetAreasAsync()
        {
            return await _db.Queryable<Area>().ToListAsync();
        }

        public async Task<List<DataModel>> GetModelsWithVariablesAsync()
        {
            return await _db.Queryable<DataModel>()
                .Includes(m => m.Variables)
                .ToListAsync();
        }

        public async Task<DeviceEntity> GetDeviceDetailAsync(int id)
        {
            return await _db.Queryable<DeviceEntity>()
                .Includes(d => d.Area)
                .Includes(d => d.Model)
                .InSingleAsync(id);
        }

        public async Task<List<DeviceEntity>> GetDevicesAsync()
        {
            return await _db.Queryable<DeviceEntity>()
                .Includes(d => d.Area)
                .Includes(d => d.Model)
                .ToListAsync();
        }
    }

    public class SqlSugarHmiRepository : IHmiRepository
    {
        private readonly ISqlSugarClient _db;

        public SqlSugarHmiRepository(ISqlSugarClient db)
        {
            _db = db;
        }

        public async Task<ScadaProject> GetProjectFullAsync(int id)
        {
            return await _db.Queryable<ScadaProject>()
                .Includes(p => p.Pages, page => page.Components)
                .InSingleAsync(id);
        }

        public async Task<ScadaPage> GetPageWithComponentsAsync(int id)
        {
            return await _db.Queryable<ScadaPage>()
                .Includes(p => p.Components)
                .InSingleAsync(id);
        }

        public async Task SavePageComponentsAsync(int pageId, List<HmiComponent> components)
        {
            await _db.Deleteable<HmiComponent>().Where(c => c.PageId == pageId).ExecuteCommandAsync();
            if (components != null && components.Count > 0)
            {
                components.ForEach(c => c.PageId = pageId);
                await _db.Insertable(components).ExecuteCommandAsync();
            }
        }
    }

    public class SqlSugarAutomationRepository : IAutomationRepository
    {
        private readonly ISqlSugarClient _db;

        public SqlSugarAutomationRepository(ISqlSugarClient db)
        {
            _db = db;
        }

        public async Task<List<VariableTrigger>> GetTriggersByDeviceAsync(int deviceId)
        {
            return await _db.Queryable<VariableTrigger>().Where(t => t.DeviceId == deviceId).ToListAsync();
        }

        public async Task<List<DataConversion>> GetActiveConversionsAsync()
        {
            return await _db.Queryable<DataConversion>().Where(c => c.Active).ToListAsync();
        }
    }

    public class SqlSugarDataRepository : IDataRepository
    {
        private readonly ISqlSugarClient _db;

        public SqlSugarDataRepository(ISqlSugarClient db)
        {
            _db = db;
        }

        public async Task<List<HistoricalRecord>> GetHistoryAsync(int deviceId, string key, DateTime start, DateTime end)
        {
            return await _db.Queryable<HistoricalRecord>()
                .Where(r => r.DeviceId == deviceId && r.VariableKey == key && r.Timestamp >= start && r.Timestamp <= end)
                .OrderBy(r => r.Timestamp)
                .ToListAsync();
        }

        public async Task<RealtimeData> GetRealtimeAsync(int deviceId, string key)
        {
            return await _db.Queryable<RealtimeData>()
                .FirstAsync(r => r.DeviceId == deviceId && r.VariableKey == key);
        }
    }
}
