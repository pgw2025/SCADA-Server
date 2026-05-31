using ScadaServer.Domain.Entities;

namespace ScadaServer.Application.Interfaces
{
    public interface IAssetRepository
    {
        Task<List<Area>> GetAreasAsync();
        Task<List<DataModel>> GetModelsWithVariablesAsync();
        Task<DeviceEntity> GetDeviceDetailAsync(int id);
        Task<List<DeviceEntity>> GetDevicesAsync();
    }

    public interface IHmiRepository
    {
        Task<ScadaProject> GetProjectFullAsync(int id);
        Task<ScadaPage> GetPageWithComponentsAsync(int id);
        Task SavePageComponentsAsync(int pageId, List<HmiComponent> components);
    }

    public interface IAutomationRepository
    {
        Task<List<VariableTrigger>> GetTriggersByDeviceAsync(int deviceId);
        Task<List<DataConversion>> GetActiveConversionsAsync();
    }

    public interface IDataRepository
    {
        Task<List<HistoricalRecord>> GetHistoryAsync(int deviceId, string key, DateTime start, DateTime end);
        Task<RealtimeData> GetRealtimeAsync(int deviceId, string key);
    }
}
