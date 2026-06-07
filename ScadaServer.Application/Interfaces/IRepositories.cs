using ScadaServer.Application.DTOs;
using ScadaServer.Domain.Entities;
using ScadaServer.Domain.Interfaces;

namespace ScadaServer.Application.Interfaces
{
    public interface IAlarmRuleRepository : IRepository<AlarmRule> { }
    public interface IAreaRepository : IRepository<Area> { }
    public interface IConfigLogRepository : IRepository<ConfigLog> { }
    public interface IDatabaseConfigRepository : IRepository<DatabaseConfig> { }
    public interface IDataConversionRepository : IRepository<DataConversion> { }
    public interface IDataModelRepository : IRepository<DataModel> { }
    public interface IExposedInterfaceRepository : IRepository<ExposedInterface> { }
    public interface IHmiComponentRepository : IRepository<HmiComponent> { }
    public interface IHistoricalRecordRepository : IRepository<HistoricalRecord> 
    { 
        Task<HistoricalRecord> GetByIdAsync(long id); 
    }
    public interface IModelVariableRepository : IRepository<ModelVariable> { }
    public interface IMqttServerRepository : IRepository<MqttServer> { }
    public interface IRealtimeDataRepository : IRepository<RealtimeData> 
    { 
        Task<RealtimeData> GetByIdAsync(int deviceId, string variableKey); 
    }
    public interface IScadaPageRepository : IRepository<ScadaPage> { }
    public interface IScadaProjectRepository : IRepository<ScadaProject> { }
    public interface IScheduledTaskRepository : IRepository<ScheduledTask> { }
    public interface ISensorRepository : IRepository<Sensor> { }
    public interface ISystemConfigRepository : IRepository<SystemConfig> { }
    public interface ISystemLogRepository : IRepository<SystemLog> { }
    public interface ISystemScriptRepository : IRepository<SystemScript> { }
    public interface ISystemUserRepository : IRepository<SystemUser> { }
    public interface IVariableTriggerRepository : IRepository<VariableTrigger> { }
}

