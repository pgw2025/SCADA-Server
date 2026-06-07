using ScadaServer.Domain.Entities;
using ScadaServer.Infrastructure.DBEntities;

namespace ScadaServer.Infrastructure.Persistence
{
    public static class EntityMapper
    {
        public static Area MapToDomain(AreaDbEntity dbEntity)
        {
            return new Area
            {
                Id = dbEntity.Id,
                Name = dbEntity.Name,
                Description = dbEntity.Description
            };
        }

        public static AreaDbEntity MapToDb(Area domainEntity)
        {
            return new AreaDbEntity
            {
                Id = domainEntity.Id,
                Name = domainEntity.Name,
                Description = domainEntity.Description
            };
        }

        public static AlarmRule MapToDomain(AlarmRuleDbEntity dbEntity)
        {
            return new AlarmRule
            {
                Id = dbEntity.Id,
                SensorId = dbEntity.SensorId,
                Condition = dbEntity.Condition,
                Threshold = dbEntity.Threshold,
                Severity = dbEntity.Severity,
                IsEnabled = dbEntity.IsEnabled
            };
        }

        public static AlarmRuleDbEntity MapToDb(AlarmRule domainEntity)
        {
            return new AlarmRuleDbEntity
            {
                Id = domainEntity.Id,
                SensorId = domainEntity.SensorId,
                Condition = domainEntity.Condition,
                Threshold = domainEntity.Threshold,
                Severity = domainEntity.Severity,
                IsEnabled = domainEntity.IsEnabled
            };
        }

        public static ConfigLog MapToDomain(ConfigLogDbEntity dbEntity)
        {
            return new ConfigLog
            {
                Id = dbEntity.Id,
                DeviceId = dbEntity.DeviceId,
                Operator = dbEntity.Operator,
                ChangeDesc = dbEntity.ChangeDesc,
                CreateTime = dbEntity.CreateTime
            };
        }

        public static ConfigLogDbEntity MapToDb(ConfigLog domainEntity)
        {
            return new ConfigLogDbEntity
            {
                Id = domainEntity.Id,
                DeviceId = domainEntity.DeviceId,
                Operator = domainEntity.Operator,
                ChangeDesc = domainEntity.ChangeDesc,
                CreateTime = domainEntity.CreateTime
            };
        }

        public static DatabaseConfig MapToDomain(DatabaseConfigDbEntity dbEntity)
        {
            return new DatabaseConfig
            {
                Id = dbEntity.Id,
                Name = dbEntity.Name,
                Type = dbEntity.Type,
                BackendType = dbEntity.BackendType,
                Host = dbEntity.Host,
                Port = dbEntity.Port,
                Username = dbEntity.Username,
                Password = dbEntity.Password,
                DatabaseName = dbEntity.DatabaseName
            };
        }

        public static DatabaseConfigDbEntity MapToDb(DatabaseConfig domainEntity)
        {
            return new DatabaseConfigDbEntity
            {
                Id = domainEntity.Id,
                Name = domainEntity.Name,
                Type = domainEntity.Type,
                BackendType = domainEntity.BackendType,
                Host = domainEntity.Host,
                Port = domainEntity.Port,
                Username = domainEntity.Username,
                Password = domainEntity.Password,
                DatabaseName = domainEntity.DatabaseName
            };
        }

        public static DataConversion MapToDomain(DataConversionDbEntity dbEntity)
        {
            return new DataConversion
            {
                Id = dbEntity.Id,
                Name = dbEntity.Name,
                SourceDeviceId = dbEntity.SourceDeviceId,
                SourceVariableKey = dbEntity.SourceVariableKey,
                TargetDeviceId = dbEntity.TargetDeviceId,
                TargetVariableKey = dbEntity.TargetVariableKey,
                Active = dbEntity.Active
            };
        }

        public static DataConversionDbEntity MapToDb(DataConversion domainEntity)
        {
            return new DataConversionDbEntity
            {
                Id = domainEntity.Id,
                Name = domainEntity.Name,
                SourceDeviceId = domainEntity.SourceDeviceId,
                SourceVariableKey = domainEntity.SourceVariableKey,
                TargetDeviceId = domainEntity.TargetDeviceId,
                TargetVariableKey = domainEntity.TargetVariableKey,
                Active = domainEntity.Active
            };
        }

        public static DataModel MapToDomain(DataModelDbEntity dbEntity)
        {
            return new DataModel
            {
                Id = dbEntity.Id,
                Name = dbEntity.Name,
                Description = dbEntity.Description,
                Type = dbEntity.Type,
                CreatedAt = dbEntity.CreatedAt,
                UpdatedAt = dbEntity.UpdatedAt
            };
        }

        public static DataModelDbEntity MapToDb(DataModel domainEntity)
        {
            return new DataModelDbEntity
            {
                Id = domainEntity.Id,
                Name = domainEntity.Name,
                Description = domainEntity.Description,
                Type = domainEntity.Type,
                CreatedAt = domainEntity.CreatedAt,
                UpdatedAt = domainEntity.UpdatedAt
            };
        }

        public static DeviceConfig MapToDomain(DeviceConfigDbEntity dbEntity)
        {
            return new DeviceConfig
            {
                DeviceId = dbEntity.DeviceId,
                JsonConfig = dbEntity.JsonConfig,
                Version = dbEntity.Version,
                UpdatedAt = dbEntity.UpdatedAt
            };
        }

        public static DeviceConfigDbEntity MapToDb(DeviceConfig domainEntity)
        {
            return new DeviceConfigDbEntity
            {
                DeviceId = domainEntity.DeviceId,
                JsonConfig = domainEntity.JsonConfig,
                Version = domainEntity.Version,
                UpdatedAt = domainEntity.UpdatedAt
            };
        }

        public static HistoricalRecord MapToDomain(HistoricalRecordDbEntity dbEntity)
        {
            return new HistoricalRecord
            {
                Id = dbEntity.Id,
                DeviceId = dbEntity.DeviceId,
                VariableKey = dbEntity.VariableKey,
                Value = dbEntity.Value,
                Timestamp = dbEntity.Timestamp
            };
        }

        public static HistoricalRecordDbEntity MapToDb(HistoricalRecord domainEntity)
        {
            return new HistoricalRecordDbEntity
            {
                Id = domainEntity.Id,
                DeviceId = domainEntity.DeviceId,
                VariableKey = domainEntity.VariableKey,
                Value = domainEntity.Value,
                Timestamp = domainEntity.Timestamp
            };
        }

        public static HmiComponent MapToDomain(HmiComponentDbEntity dbEntity)
        {
            return new HmiComponent
            {
                Id = dbEntity.Id,
                PageId = dbEntity.PageId,
                Type = dbEntity.Type,
                Name = dbEntity.Name,
                X = dbEntity.X,
                Y = dbEntity.Y,
                Width = dbEntity.Width,
                Height = dbEntity.Height,
                ZIndex = dbEntity.ZIndex,
                BindField = dbEntity.BindField,
                PropsJson = dbEntity.PropsJson
            };
        }

        public static HmiComponentDbEntity MapToDb(HmiComponent domainEntity)
        {
            return new HmiComponentDbEntity
            {
                Id = domainEntity.Id,
                PageId = domainEntity.PageId,
                Type = domainEntity.Type,
                Name = domainEntity.Name,
                X = domainEntity.X,
                Y = domainEntity.Y,
                Width = domainEntity.Width,
                Height = domainEntity.Height,
                ZIndex = domainEntity.ZIndex,
                BindField = domainEntity.BindField,
                PropsJson = domainEntity.PropsJson
            };
        }

        public static ModelVariable MapToDomain(ModelVariableDbEntity dbEntity)
        {
            return new ModelVariable
            {
                Id = dbEntity.Id,
                ModelId = dbEntity.ModelId,
                Key = dbEntity.Key,
                Name = dbEntity.Name,
                Type = dbEntity.Type,
                DataType = dbEntity.DataType,
                Unit = dbEntity.Unit,
                Min = dbEntity.Min,
                Max = dbEntity.Max,
                Address = dbEntity.Address,
                Description = dbEntity.Description,
                IsStored = dbEntity.IsStored,
                StoreMode = dbEntity.StoreMode,
                UpdateMode = dbEntity.UpdateMode,
                PollingIntervalMs = dbEntity.PollingIntervalMs,
                BitOffset = dbEntity.BitOffset,
                ScaleSlope = dbEntity.ScaleSlope,
                ScaleOffset = dbEntity.ScaleOffset,
                DeadBand = dbEntity.DeadBand,
                IsReadOnly = dbEntity.IsReadOnly,
                ExtensionData = dbEntity.ExtensionData
            };
        }

        public static ModelVariableDbEntity MapToDb(ModelVariable domainEntity)
        {
            return new ModelVariableDbEntity
            {
                Id = domainEntity.Id,
                ModelId = domainEntity.ModelId,
                Key = domainEntity.Key,
                Name = domainEntity.Name,
                Type = domainEntity.Type,
                DataType = domainEntity.DataType,
                Unit = domainEntity.Unit,
                Min = domainEntity.Min,
                Max = domainEntity.Max,
                Address = domainEntity.Address,
                Description = domainEntity.Description,
                IsStored = domainEntity.IsStored,
                StoreMode = domainEntity.StoreMode,
                UpdateMode = domainEntity.UpdateMode,
                PollingIntervalMs = domainEntity.PollingIntervalMs,
                BitOffset = domainEntity.BitOffset,
                ScaleSlope = domainEntity.ScaleSlope,
                ScaleOffset = domainEntity.ScaleOffset,
                DeadBand = domainEntity.DeadBand,
                IsReadOnly = domainEntity.IsReadOnly,
                ExtensionData = domainEntity.ExtensionData
            };
        }

        public static MqttServer MapToDomain(MqttServerDbEntity dbEntity)
        {
            return new MqttServer
            {
                Id = dbEntity.Id,
                Name = dbEntity.Name,
                BrokerUrl = dbEntity.BrokerUrl,
                Port = dbEntity.Port,
                ClientId = dbEntity.ClientId,
                Username = dbEntity.Username,
                Password = dbEntity.Password,
                TopicPrefix = dbEntity.TopicPrefix
            };
        }

        public static MqttServerDbEntity MapToDb(MqttServer domainEntity)
        {
            return new MqttServerDbEntity
            {
                Id = domainEntity.Id,
                Name = domainEntity.Name,
                BrokerUrl = domainEntity.BrokerUrl,
                Port = domainEntity.Port,
                ClientId = domainEntity.ClientId,
                Username = domainEntity.Username,
                Password = domainEntity.Password,
                TopicPrefix = domainEntity.TopicPrefix
            };
        }

        public static MqttVariableConfig MapToDomain(MqttVariableConfigDbEntity dbEntity)
        {
            return new MqttVariableConfig
            {
                Id = dbEntity.Id,
                MqttServerId = dbEntity.MqttServerId,
                DeviceId = dbEntity.DeviceId,
                VariableKey = dbEntity.VariableKey,
                Alias = dbEntity.Alias,
                IsEnabled = dbEntity.IsEnabled,
                CustomTopic = dbEntity.CustomTopic
            };
        }

        public static MqttVariableConfigDbEntity MapToDb(MqttVariableConfig domainEntity)
        {
            return new MqttVariableConfigDbEntity
            {
                Id = domainEntity.Id,
                MqttServerId = domainEntity.MqttServerId,
                DeviceId = domainEntity.DeviceId,
                VariableKey = domainEntity.VariableKey,
                Alias = domainEntity.Alias,
                IsEnabled = domainEntity.IsEnabled,
                CustomTopic = domainEntity.CustomTopic
            };
        }

        public static RealtimeData MapToDomain(RealtimeDataDbEntity dbEntity)
        {
            return new RealtimeData
            {
                DeviceId = dbEntity.DeviceId,
                VariableKey = dbEntity.VariableKey,
                Value = dbEntity.Value,
                Timestamp = dbEntity.Timestamp
            };
        }

        public static RealtimeDataDbEntity MapToDb(RealtimeData domainEntity)
        {
            return new RealtimeDataDbEntity
            {
                DeviceId = domainEntity.DeviceId,
                VariableKey = domainEntity.VariableKey,
                Value = domainEntity.Value,
                Timestamp = domainEntity.Timestamp
            };
        }

        public static ScheduledTask MapToDomain(ScheduledTaskDbEntity dbEntity)
        {
            return new ScheduledTask
            {
                Id = dbEntity.Id,
                Name = dbEntity.Name,
                Type = dbEntity.Type,
                CronExpression = dbEntity.CronExpression,
                ParamsJson = dbEntity.ParamsJson,
                Active = dbEntity.Active
            };
        }

        public static ScheduledTaskDbEntity MapToDb(ScheduledTask domainEntity)
        {
            return new ScheduledTaskDbEntity
            {
                Id = domainEntity.Id,
                Name = domainEntity.Name,
                Type = domainEntity.Type,
                CronExpression = domainEntity.CronExpression,
                ParamsJson = domainEntity.ParamsJson,
                Active = domainEntity.Active
            };
        }

        public static ScadaPage MapToDomain(ScadaPageDbEntity dbEntity)
        {
            return new ScadaPage
            {
                Id = dbEntity.Id,
                ProjectId = dbEntity.ProjectId,
                Name = dbEntity.Name,
                IsHome = dbEntity.IsHome
            };
        }

        public static ScadaPageDbEntity MapToDb(ScadaPage domainEntity)
        {
            return new ScadaPageDbEntity
            {
                Id = domainEntity.Id,
                ProjectId = domainEntity.ProjectId,
                Name = domainEntity.Name,
                IsHome = domainEntity.IsHome
            };
        }

        public static ScadaProject MapToDomain(ScadaProjectDbEntity dbEntity)
        {
            return new ScadaProject
            {
                Id = dbEntity.Id,
                Name = dbEntity.Name,
                Description = dbEntity.Description,
                CreatedAt = dbEntity.CreatedAt
            };
        }

        public static ScadaProjectDbEntity MapToDb(ScadaProject domainEntity)
        {
            return new ScadaProjectDbEntity
            {
                Id = domainEntity.Id,
                Name = domainEntity.Name,
                Description = domainEntity.Description,
                CreatedAt = domainEntity.CreatedAt
            };
        }

        public static SystemConfig MapToDomain(SystemConfigDbEntity dbEntity)
        {
            return new SystemConfig
            {
                Id = dbEntity.Id,
                SystemTitle = dbEntity.SystemTitle,
                PollIntervalMs = dbEntity.PollIntervalMs,
                MqttBrokerHost = dbEntity.MqttBrokerHost,
                RetentionPeriodDays = dbEntity.RetentionPeriodDays
            };
        }

        public static SystemConfigDbEntity MapToDb(SystemConfig domainEntity)
        {
            return new SystemConfigDbEntity
            {
                Id = domainEntity.Id,
                SystemTitle = domainEntity.SystemTitle,
                PollIntervalMs = domainEntity.PollIntervalMs,
                MqttBrokerHost = domainEntity.MqttBrokerHost,
                RetentionPeriodDays = domainEntity.RetentionPeriodDays
            };
        }

        public static SystemLog MapToDomain(SystemLogDbEntity dbEntity)
        {
            return new SystemLog
            {
                Id = dbEntity.Id,
                Timestamp = dbEntity.Timestamp,
                Level = dbEntity.Level,
                Source = dbEntity.Source,
                Content = dbEntity.Content
            };
        }

        public static SystemLogDbEntity MapToDb(SystemLog domainEntity)
        {
            return new SystemLogDbEntity
            {
                Id = domainEntity.Id,
                Timestamp = domainEntity.Timestamp,
                Level = domainEntity.Level,
                Source = domainEntity.Source,
                Content = domainEntity.Content
            };
        }

        public static SystemScript MapToDomain(SystemScriptDbEntity dbEntity)
        {
            return new SystemScript
            {
                Id = dbEntity.Id,
                Name = dbEntity.Name,
                Code = dbEntity.Code,
                TriggerType = dbEntity.TriggerType,
                IntervalSeconds = dbEntity.IntervalSeconds,
                Active = dbEntity.Active
            };
        }

        public static SystemScriptDbEntity MapToDb(SystemScript domainEntity)
        {
            return new SystemScriptDbEntity
            {
                Id = domainEntity.Id,
                Name = domainEntity.Name,
                Code = domainEntity.Code,
                TriggerType = domainEntity.TriggerType,
                IntervalSeconds = domainEntity.IntervalSeconds,
                Active = domainEntity.Active
            };
        }

        public static SystemUser MapToDomain(SystemUserDbEntity dbEntity)
        {
            return new SystemUser
            {
                Id = dbEntity.Id,
                Username = dbEntity.Username,
                PasswordHash = dbEntity.PasswordHash,
                Role = dbEntity.Role,
                Status = dbEntity.Status
            };
        }

        public static SystemUserDbEntity MapToDb(SystemUser domainEntity)
        {
            return new SystemUserDbEntity
            {
                Id = domainEntity.Id,
                Username = domainEntity.Username,
                PasswordHash = domainEntity.PasswordHash,
                Role = domainEntity.Role,
                Status = domainEntity.Status
            };
        }

        public static VariableTrigger MapToDomain(VariableTriggerDbEntity dbEntity)
        {
            return new VariableTrigger
            {
                Id = dbEntity.Id,
                Name = dbEntity.Name,
                DeviceId = dbEntity.DeviceId,
                VariableKey = dbEntity.VariableKey,
                Condition = dbEntity.Condition,
                Threshold = dbEntity.Threshold,
                ActionType = dbEntity.ActionType,
                AlarmLevel = dbEntity.AlarmLevel,
                LinkageVariableKey = dbEntity.LinkageVariableKey,
                LinkageValue = dbEntity.LinkageValue,
                Active = dbEntity.Active
            };
        }

        public static VariableTriggerDbEntity MapToDb(VariableTrigger domainEntity)
        {
            return new VariableTriggerDbEntity
            {
                Id = domainEntity.Id,
                Name = domainEntity.Name,
                DeviceId = domainEntity.DeviceId,
                VariableKey = domainEntity.VariableKey,
                Condition = domainEntity.Condition,
                Threshold = domainEntity.Threshold,
                ActionType = domainEntity.ActionType,
                AlarmLevel = domainEntity.AlarmLevel,
                LinkageVariableKey = domainEntity.LinkageVariableKey,
                LinkageValue = domainEntity.LinkageValue,
                Active = domainEntity.Active
            };
        }

        public static Sensor MapToDomain(SensorDbEntity dbEntity)
        {
            return new Sensor
            {
                Id = dbEntity.Id,
                DeviceId = dbEntity.DeviceId,
                VariableKey = dbEntity.VariableKey,
                Name = dbEntity.Name,
                Unit = dbEntity.Unit,
                LastValue = dbEntity.LastValue,
                LastUpdateTime = dbEntity.LastUpdateTime
            };
        }

        public static SensorDbEntity MapToDb(Sensor domainEntity)
        {
            return new SensorDbEntity
            {
                Id = domainEntity.Id,
                DeviceId = domainEntity.DeviceId,
                VariableKey = domainEntity.VariableKey,
                Name = domainEntity.Name,
                Unit = domainEntity.Unit,
                LastValue = domainEntity.LastValue,
                LastUpdateTime = domainEntity.LastUpdateTime
            };
        }

        public static ExposedInterface MapToDomain(ExposedInterfaceDbEntity dbEntity)
        {
            return new ExposedInterface
            {
                Id = dbEntity.Id,
                Name = dbEntity.Name,
                RouteUrl = dbEntity.RouteUrl,
                RequestMethod = dbEntity.RequestMethod,
                DeviceId = dbEntity.DeviceId,
                ExposedKey = dbEntity.ExposedKey,
                Active = dbEntity.Active
            };
        }

        public static ExposedInterfaceDbEntity MapToDb(ExposedInterface domainEntity)
        {
            return new ExposedInterfaceDbEntity
            {
                Id = domainEntity.Id,
                Name = domainEntity.Name,
                RouteUrl = domainEntity.RouteUrl,
                RequestMethod = domainEntity.RequestMethod,
                DeviceId = domainEntity.DeviceId,
                ExposedKey = domainEntity.ExposedKey,
                Active = domainEntity.Active
            };
        }

        public static Device MapToDomain(DeviceDbEntity dbEntity)
        {
            return new Device
            {
                Id = dbEntity.Id,
                Name = dbEntity.Name,
                Key = dbEntity.Key,
                AreaId = dbEntity.AreaId,
                Area = dbEntity.Area == null ? null : MapToDomain(dbEntity.Area),
                ModelId = dbEntity.ModelId,
                Model = dbEntity.Model == null ? null : MapToDomain(dbEntity.Model),
                Type = dbEntity.Type,
                IsEnabled = dbEntity.IsEnabled,
                PollingInterval = dbEntity.PollingInterval,
                DriverName = dbEntity.DriverName,
                CreatedAt = dbEntity.CreatedAt,
                UpdatedAt = dbEntity.UpdatedAt,
                LastCommunicationTime = dbEntity.LastCommunicationTime,
                Config = dbEntity.Config == null ? null : MapToDomain(dbEntity.Config),
                Triggers = dbEntity.Triggers == null ? null : dbEntity.Triggers.Select(MapToDomain).ToList()
            };
        }

        public static DeviceDbEntity MapToDb(Device domainEntity)
        {
            return new DeviceDbEntity
            {
                Id = domainEntity.Id,
                Name = domainEntity.Name,
                Key = domainEntity.Key,
                AreaId = domainEntity.AreaId,
                ModelId = domainEntity.ModelId,
                Type = domainEntity.Type,
                IsEnabled = domainEntity.IsEnabled,
                PollingInterval = domainEntity.PollingInterval,
                DriverName = domainEntity.DriverName,
                CreatedAt = domainEntity.CreatedAt,
                UpdatedAt = domainEntity.UpdatedAt,
                LastCommunicationTime = domainEntity.LastCommunicationTime
            };
        }
    }
}