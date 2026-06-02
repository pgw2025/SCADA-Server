using ScadaServer.Domain.Enums;

namespace ScadaServer.Application.DTOs;

public class ModelVariableDto
{
    public int Id { get; set; }
    public int ModelId { get; set; }
    public string Key { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string DataType { get; set; } = string.Empty;
    public string? Unit { get; set; }
    public double? Min { get; set; }
    public double? Max { get; set; }
    public string Address { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsStored { get; set; }
    public string StoreMode { get; set; } = string.Empty;
    public UpdateMode UpdateMode { get; set; }
    public int PollingIntervalMs { get; set; } = 1000;
    public Dictionary<string, string>? ExtensionData { get; set; }
}
