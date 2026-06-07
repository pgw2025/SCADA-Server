using ScadaServer.Domain.Entities;

namespace ScadaServer.Domain.Interfaces.Repositories
{
    public interface IAlarmRuleRepository : IRepository<AlarmRule, int> { }
    public interface IAreaRepository : IRepository<Area, int> { }
    public interface IConfigLogRepository : IRepository<ConfigLog, int> { }
    public interface IDatabaseConfigRepository : IRepository<DatabaseConfig, int> { }
    public interface IDataConversionRepository : IRepository<DataConversion, int> { }
    public interface IDeviceRepository : IRepository<Device, int> { }
    public interface IDataModelRepository : IRepository<DataModel, int> { }
    public interface IExposedInterfaceRepository : IRepository<ExposedInterface, int> { }
    public interface IHmiComponentRepository : IRepository<HmiComponent, int> { }
    public interface IModelVariableRepository : IRepository<ModelVariable, int> { }
    public interface IMqttServerRepository : IRepository<MqttServer, int> { }
    public interface IScadaPageRepository : IRepository<ScadaPage, int> { }
    public interface IScadaProjectRepository : IRepository<ScadaProject, int> { }
    public interface IScheduledTaskRepository : IRepository<ScheduledTask, int> { }
    public interface ISensorRepository : IRepository<Sensor, int> { }
    public interface ISystemConfigRepository : IRepository<SystemConfig, int> { }
    public interface ISystemLogRepository : IRepository<SystemLog, int> { }
    public interface ISystemScriptRepository : IRepository<SystemScript, int> { }
    public interface ISystemUserRepository : IRepository<SystemUser, int> { }
    public interface IVariableTriggerRepository : IRepository<VariableTrigger, int> { }
}
