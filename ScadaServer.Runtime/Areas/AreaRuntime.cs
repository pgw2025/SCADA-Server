using System.Collections.Concurrent;
using ScadaServer.Domain.Entities;

namespace ScadaServer.Runtime.Areas;

public class AreaRuntime
{
    /// <summary>
    /// 区域实体
    /// </summary>
    public Area Area { get; }

    /// <summary>
    /// 区域ID
    /// </summary>
    public int Id => Area.Id;

    /// <summary>
    /// 区域名称
    /// </summary>
    public string Name => Area.Name;

    /// <summary>
    /// 区域下所有设备
    /// </summary>
    public ConcurrentDictionary<int, DeviceRuntime> Devices { get; }

    /// <summary>
    /// 区域状态
    /// </summary>
    public RuntimeStatus Status { get; set; }

    /// <summary>
    /// 是否启用
    /// </summary>
    public bool IsEnabled => Area.IsEnabled;

    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreateTime { get; }

    /// <summary>
    /// 最后更新时间
    /// </summary>
    public DateTime LastUpdateTime { get; private set; }

    /// <summary>
    /// 区域统计信息
    /// </summary>
    public AreaStatistics Statistics { get; } = new();

    public AreaRuntime(Area area)
    {
        Area = area;

        Devices = new ConcurrentDictionary<int, DeviceRuntime>();

        CreateTime = DateTime.Now;

        LastUpdateTime = DateTime.Now;

        Status = RuntimeStatus.Stopped;
    }

    public bool AddDevice(DeviceRuntime runtime)
    {
        return Devices.TryAdd(runtime.Id, runtime);
    }

    public bool RemoveDevice(int deviceId)
    {
        return Devices.TryRemove(deviceId, out _);
    }

    public void UpdateTime()
    {
        LastUpdateTime = DateTime.Now;
    }
}