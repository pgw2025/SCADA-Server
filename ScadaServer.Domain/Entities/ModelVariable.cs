using SqlSugar;
using ScadaServer.Domain.Enums;

namespace ScadaServer.Domain.Entities
{
    [SugarTable("ModelVariables")]
    public class ModelVariable : EntityBase
    {
        public int ModelId { get; set; }
        public string Key { get; set; }
        public string Name { get; set; }

        public VariableType Type { get; set; }
        public DataTypeEnum DataType { get; set; }

        [SugarColumn(IsNullable = true)]
        public string? Unit { get; set; }
        [SugarColumn(IsNullable = true)]
        public double? Min { get; set; }
        [SugarColumn(IsNullable = true)]
        public double? Max { get; set; }
        public string Address { get; set; }
        [SugarColumn(IsNullable = true)]
        public string? Description { get; set; }
        public bool IsStored { get; set; }
        public string StoreMode { get; set; }

        public UpdateMode UpdateMode { get; set; }
        public int PollingIntervalMs { get; set; } = 1000;

        [SugarColumn(IsNullable = true)]
        public int? BitOffset { get; set; }

        public double ScaleSlope { get; set; } = 1.0;

        public double ScaleOffset { get; set; } = 0.0;

        [SugarColumn(IsNullable = true)]
        public double? DeadBand { get; set; }

        public bool IsReadOnly { get; set; } = true;

        [SugarColumn(ColumnDataType = "longtext", IsJson = true)]
        public Dictionary<string, string>? ExtensionData { get; set; }
    }
}