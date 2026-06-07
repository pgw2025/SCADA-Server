using ScadaServer.Domain.Entities;
using ScadaServer.Domain.Enums;

namespace ScadaServer.Infrastructure.Communication
{
    public interface IProtocolDriverFactory
    {
        IProtocolDriver CreateDriver(DeviceType deviceType);
        IProtocolDriver CreateDriver(string driverName);
    }

    public class ProtocolDriverFactory : IProtocolDriverFactory
    {
        public IProtocolDriver CreateDriver(DeviceType deviceType)
        {
            return deviceType switch
            {
                DeviceType.S7 => new S7Driver(),
                DeviceType.OpcUa => new OpcUaDriver(),
                DeviceType.ModbusTcp => throw new NotSupportedException($"驱动 {deviceType} 尚未实现"),
                DeviceType.Mqtt => throw new NotSupportedException($"驱动 {deviceType} 尚未实现"),
                DeviceType.Virtual => throw new NotSupportedException($"驱动 {deviceType} 尚未实现"),
                _ => throw new NotSupportedException($"不支持的设备类型: {deviceType}")
            };
        }

        public IProtocolDriver CreateDriver(string driverName)
        {
            return driverName?.ToUpper() switch
            {
                "S7DRIVER" or "S7" => new S7Driver(),
                "OPCUADRIVER" or "OPCUA" => new OpcUaDriver(),
                "MODBUSTCPDRIVER" or "MODBUSTCP" => throw new NotSupportedException($"驱动 {driverName} 尚未实现"),
                "MQTTDRIVER" or "MQTT" => throw new NotSupportedException($"驱动 {driverName} 尚未实现"),
                "VIRTUALDRIVER" or "VIRTUAL" => throw new NotSupportedException($"驱动 {driverName} 尚未实现"),
                _ => throw new NotSupportedException($"不支持的驱动名称: {driverName}")
            };
        }
    }
}
