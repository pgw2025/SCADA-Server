using ScadaServer.Domain.Enums;

namespace ScadaServer.Application.DTOs
{
    // --- 核心资产 (Asset) ---
    
    public class AreaDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }

    public class DataModelDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Type { get; set; }
        public List<ModelVariableDto> Variables { get; set; }
    }

    public class ModelVariableDto
    {
        public int Id { get; set; }
        public string Key { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string DataType { get; set; }
        public string Unit { get; set; }
        public string Address { get; set; }
        public bool IsStored { get; set; }
    }

    public class DeviceDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string AreaName { get; set; }
        public string ModelName { get; set; }
        public string Type { get; set; }
        public string IpAddress { get; set; }
        public DeviceStatus Status { get; set; }
        public DateTime LastUpdated { get; set; }
    }

    // --- HMI 组态 ---

    public class ScadaProjectDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<ScadaPageDto> Pages { get; set; }
    }

    public class ScadaPageDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsHome { get; set; }
        public List<HmiComponentDto> Components { get; set; }
    }

    public class HmiComponentDto
    {
        public int Id { get; set; }
        public string Type { get; set; }
        public string Name { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public string BindField { get; set; }
        public string PropsJson { get; set; }
    }

    // --- 自动化与告警 ---

    public class TriggerDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string DeviceName { get; set; }
        public string VariableKey { get; set; }
        public string Condition { get; set; }
        public double Threshold { get; set; }
        public string AlarmLevel { get; set; }
        public bool Active { get; set; }
    }

    // --- 系统 ---

    public class UserDto
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Role { get; set; }
    }
}
