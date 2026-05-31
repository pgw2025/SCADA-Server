using System.Collections.Concurrent;
using ScadaServer.Domain.Entities;

namespace ScadaServer.Infrastructure.Communication
{
    // 线程安全的设备配置运行时注册表
    public class DeviceRegistry
    {
        private readonly ConcurrentDictionary<int, (DeviceEntity Device, List<ModelVariable> Variables)> _deviceCache = new();

        public void UpdateDevice(DeviceEntity device, List<ModelVariable> variables)
        {
            _deviceCache[device.Id] = (device, variables);
        }

        public void RemoveDevice(int deviceId)
        {
            _deviceCache.TryRemove(deviceId, out _);
        }

        public (DeviceEntity Device, List<ModelVariable> Variables)? GetDeviceConfig(int deviceId)
        {
            if (_deviceCache.TryGetValue(deviceId, out var config))
            {
                return config;
            }
            return null;
        }

        public IEnumerable<int> GetAllDeviceIds() => _deviceCache.Keys;
    }
}
