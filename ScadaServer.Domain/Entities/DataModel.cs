using SqlSugar;
using ScadaServer.Domain.Enums;

namespace ScadaServer.Domain.Entities
{
    [SugarTable("DataModels")]
        public class DataModel : EntityBase
        {
            [SugarColumn(Length = 100, IsNullable = false)]
            public string Name { get; set; } = string.Empty;

            [SugarColumn(Length = 500)]
            public string? Description { get; set; }

            public DeviceType Type { get; set; }

            public DateTime CreatedAt { get; set; } = DateTime.Now;

            public DateTime UpdatedAt { get; set; } = DateTime.Now;

            [SugarColumn(IsIgnore = true)]
            public List<ModelVariable>? Variables { get; set; }
        }
}