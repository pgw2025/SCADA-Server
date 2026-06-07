using ScadaServer.Domain.Enums;

namespace ScadaServer.Domain.Entities
{
    public class ModelVariable
    {
        public int Id { get; set; }
        public int ModelId { get; set; }
        public string Key { get; set; }
        public string Name { get; set; }
        
        public VariableType Type { get; set; }
        public DataTypeEnum DataType { get; set; }
        
        public string? Unit { get; set; }
        public double? Min { get; set; }
        public double? Max { get; set; }
        public string Address { get; set; }
        public string? Description { get; set; }
        public bool IsStored { get; set; }
        public string StoreMode { get; set; }
        public UpdateMode UpdateMode { get; set; }
        public int PollingIntervalMs { get; set; } = 1000;
        public int? BitOffset { get; set; }
        public double ScaleSlope { get; set; } = 1.0;
        public double ScaleOffset { get; set; } = 0.0;
        public double? DeadBand { get; set; }
        public bool IsReadOnly { get; set; } = true;
        public Dictionary<string, string>? ExtensionData { get; set; }
    }
}