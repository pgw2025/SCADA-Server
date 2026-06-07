using SqlSugar;

namespace ScadaServer.Domain.Entities
{
    [SugarTable("DeviceConfigs")]
    public class DeviceConfig
    {
        [SugarColumn(IsPrimaryKey = true)]
        public int DeviceId { get; set; }

        [SugarColumn(ColumnDataType = "nvarchar(max)")]
        public string JsonConfig { get; set; } = string.Empty;

        public int Version { get; set; } = 1;

        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }
}