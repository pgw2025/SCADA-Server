using SqlSugar;
using ScadaServer.Domain.Enums;

namespace ScadaServer.Infrastructure.DBEntities
{
    /// <summary>
    /// 设备数据库实体
    /// </summary>
    [SugarTable("Devices")]
    public class DeviceDbEntity
    {
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int Id { get; set; }

        [SugarColumn(Length = 100, IsNullable = false)]
        public string Name { get; set; } = string.Empty;

        [SugarColumn(Length = 100, IsNullable = false, ColumnDescription = "唯一键")]
        public string Key { get; set; } = string.Empty;

        public int AreaId { get; set; }

        [Navigate(NavigateType.OneToOne, nameof(AreaId))]
        public AreaDbEntity? Area { get; set; }

        public int ModelId { get; set; }

        [Navigate(NavigateType.OneToOne, nameof(ModelId))]
        public DataModelDbEntity? Model { get; set; }

        public DeviceType Type { get; set; }

        public bool IsEnabled { get; set; } = true;

        public int PollingInterval { get; set; } = 1000;

        [SugarColumn(Length = 100)]
        public string? DriverName { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        public DateTime? LastCommunicationTime { get; set; }

        [Navigate(NavigateType.OneToOne, nameof(Id), nameof(DeviceConfigDbEntity.DeviceId))]
        public DeviceConfigDbEntity? Config { get; set; }

        [Navigate(NavigateType.OneToMany, nameof(VariableTriggerDbEntity.DeviceId))]
        public List<VariableTriggerDbEntity>? Triggers { get; set; }
    }
}