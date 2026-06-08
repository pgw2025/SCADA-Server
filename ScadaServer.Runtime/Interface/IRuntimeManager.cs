namespace ScadaServer.Runtime.Interface;

using ScadaServer.Domain.Entities;
using ScadaServer.Runtime.Devices;
using ScadaServer.Runtime.Variables;

/// <summary>
/// 运行时管理器接口
/// </summary>
public interface IRuntimeManager
{
    /// <summary>
    /// 初始化运行时
    /// </summary>
    Task InitializeAsync();

    /// <summary>
    /// 启动运行时
    /// </summary>
    /// <param name="token">取消令牌</param>
    /// <param name="maxConcurrentWorkers">最大并发工作线程数</param>
    Task StartAsync(CancellationToken token, int maxConcurrentWorkers = 10);

    /// <summary>
    /// 停止运行时
    /// </summary>
    Task StopAsync();

    /// <summary>
    /// 重载配置
    /// </summary>
    Task ReloadConfigAsync();

    /// <summary>
    /// 获取所有设备运行时
    /// </summary>
    /// <returns>设备运行时列表</returns>
    IEnumerable<DeviceRuntime> GetDevices();

    /// <summary>
    /// 根据ID获取设备运行时
    /// </summary>
    /// <param name="deviceId">设备ID</param>
    /// <returns>设备运行时，不存在则返回null</returns>
    DeviceRuntime? GetDevice(int deviceId);

    /// <summary>
    /// 根据Key获取设备运行时
    /// </summary>
    /// <param name="deviceKey">设备Key</param>
    /// <returns>设备运行时，不存在则返回null</returns>
    DeviceRuntime? GetDevice(string deviceKey);

    /// <summary>
    /// 获取设备下的所有变量运行时
    /// </summary>
    /// <param name="deviceId">设备ID</param>
    /// <returns>变量运行时列表</returns>
    IEnumerable<VariableRuntime> GetVariables(int deviceId);

    /// <summary>
    /// 获取设备下的指定变量运行时
    /// </summary>
    /// <param name="deviceId">设备ID</param>
    /// <param name="variableId">变量ID</param>
    /// <returns>变量运行时，不存在则返回null</returns>
    VariableRuntime? GetVariable(int deviceId, int variableId);

    /// <summary>
    /// 添加设备
    /// </summary>
    /// <param name="device">设备实体</param>
    /// <returns>添加成功返回true，否则返回false</returns>
    Task<bool> AddDeviceAsync(Device device);

    /// <summary>
    /// 删除设备
    /// </summary>
    /// <param name="deviceId">设备ID</param>
    /// <returns>删除成功返回true，否则返回false</returns>
    Task<bool> RemoveDeviceAsync(int deviceId);

    /// <summary>
    /// 更新设备
    /// </summary>
    /// <param name="device">设备实体</param>
    /// <returns>更新成功返回true，否则返回false</returns>
    Task<bool> UpdateDeviceAsync(Device device);
}
