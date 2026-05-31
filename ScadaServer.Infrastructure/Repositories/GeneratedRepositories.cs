using SqlSugar;
using ScadaServer.Domain.Entities;
using ScadaServer.Infrastructure.Persistence;

namespace ScadaServer.Infrastructure.Repositories
{
    // 为每个实体生成简易的 Repository，继承通用的 SqlSugarRepository
    
    public class AlarmRuleRepository : SqlSugarRepository<AlarmRule> { public AlarmRuleRepository(ISqlSugarClient db) : base(db) { } }
    public class AreaRepository : SqlSugarRepository<Area> { public AreaRepository(ISqlSugarClient db) : base(db) { } }
    public class ConfigLogRepository : SqlSugarRepository<ConfigLog> { public ConfigLogRepository(ISqlSugarClient db) : base(db) { } }
    public class DatabaseConfigRepository : SqlSugarRepository<DatabaseConfig> { public DatabaseConfigRepository(ISqlSugarClient db) : base(db) { } }
    public class DataConversionRepository : SqlSugarRepository<DataConversion> { public DataConversionRepository(ISqlSugarClient db) : base(db) { } }
    public class DataModelRepository : SqlSugarRepository<DataModel> { public DataModelRepository(ISqlSugarClient db) : base(db) { } }
    public class DeviceRepository : SqlSugarRepository<Device> { public DeviceRepository(ISqlSugarClient db) : base(db) { } }
    public class ExposedInterfaceRepository : SqlSugarRepository<ExposedInterface> { public ExposedInterfaceRepository(ISqlSugarClient db) : base(db) { } }
    public class HistoricalRecordRepository : SqlSugarRepository<HistoricalRecord> { public HistoricalRecordRepository(ISqlSugarClient db) : base(db) { } }
    public class HmiComponentRepository : SqlSugarRepository<HmiComponent> { public HmiComponentRepository(ISqlSugarClient db) : base(db) { } }
    public class ModelVariableRepository : SqlSugarRepository<ModelVariable> { public ModelVariableRepository(ISqlSugarClient db) : base(db) { } }
    public class MqttServerRepository : SqlSugarRepository<MqttServer> { public MqttServerRepository(ISqlSugarClient db) : base(db) { } }
    public class RealtimeDataRepository : SqlSugarRepository<RealtimeData> { public RealtimeDataRepository(ISqlSugarClient db) : base(db) { } }
    public class ScadaPageRepository : SqlSugarRepository<ScadaPage> { public ScadaPageRepository(ISqlSugarClient db) : base(db) { } }
    public class ScadaProjectRepository : SqlSugarRepository<ScadaProject> { public ScadaProjectRepository(ISqlSugarClient db) : base(db) { } }
    public class ScheduledTaskRepository : SqlSugarRepository<ScheduledTask> { public ScheduledTaskRepository(ISqlSugarClient db) : base(db) { } }
    public class SensorRepository : SqlSugarRepository<Sensor> { public SensorRepository(ISqlSugarClient db) : base(db) { } }
    public class SystemConfigRepository : SqlSugarRepository<SystemConfig> { public SystemConfigRepository(ISqlSugarClient db) : base(db) { } }
    public class SystemLogRepository : SqlSugarRepository<SystemLog> { public SystemLogRepository(ISqlSugarClient db) : base(db) { } }
    public class SystemScriptRepository : SqlSugarRepository<SystemScript> { public SystemScriptRepository(ISqlSugarClient db) : base(db) { } }
    public class SystemUserRepository : SqlSugarRepository<SystemUser> { public SystemUserRepository(ISqlSugarClient db) : base(db) { } }
    public class VariableTriggerRepository : SqlSugarRepository<VariableTrigger> { public VariableTriggerRepository(ISqlSugarClient db) : base(db) { } }
}
