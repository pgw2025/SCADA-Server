using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using ScadaServer.Domain.Enums;

namespace ScadaServer.Runtime.Devices
{
    /// <summary>
    /// 设备调度器，负责控制设备轮询频率和并发执行
    /// </summary>
    /// <remarks>
    /// 采用轮询式调度模式，以固定间隔遍历所有设备并分发工作线程。
    /// 通过 SemaphoreSlim 实现并发限流，防止过多设备同时访问导致资源耗尽。
    /// 每个设备的工作线程独立执行（fire-and-forget），调度器不等待单个设备完成即可继续调度下一个。
    /// </remarks>
    public class DeviceScheduler
    {
        private readonly RuntimeManager _runtimeManager;
        private readonly SemaphoreSlim _workerLimiter;
        private readonly ILogger<DeviceScheduler> _logger;
        private readonly ILogger<DeviceWorker> _workerLogger;

        private CancellationTokenSource? _cts;
        private Task? _schedulerTask;
        private bool _isRunning;

        /// <summary>
        /// 设备ID到工作任务的映射
        /// </summary>
        private readonly ConcurrentDictionary<int, DeviceWorkerInfo> _deviceWorkers = new();

        /// <summary>
        /// 调度器是否正在运行
        /// </summary>
        public bool IsRunning => _isRunning;

        /// <summary>
        /// 初始化设备调度器
        /// </summary>
        /// <param name="runtimeManager">运行时管理器，提供设备运行时列表</param>
        /// <param name="maxConcurrentWorkers">最大并发工作线程数，限制同时执行的设备任务数量</param>
        /// <param name="logger">日志记录器</param>
        public DeviceScheduler(RuntimeManager runtimeManager, int maxConcurrentWorkers, ILogger<DeviceScheduler> logger, ILogger<DeviceWorker> workerLogger)
        {
            _runtimeManager = runtimeManager;
            _workerLimiter = new SemaphoreSlim(maxConcurrentWorkers);
            _logger = logger;
            _workerLogger = workerLogger;
        }

        /// <summary>
        /// 启动调度器
        /// </summary>
        /// <param name="token">取消令牌</param>
        public async Task StartAsync(CancellationToken token = default)
        {
            if (_isRunning)
            {
                _logger.LogWarning("调度器已在运行中");
                return;
            }

            _cts = CancellationTokenSource.CreateLinkedTokenSource(token);
            _isRunning = true;

            _logger.LogInformation("正在启动设备调度器...");

            // 启动所有已注册的设备
            foreach (var runtime in _runtimeManager.DeviceRuntimes.Values)
            {
                StartDeviceInternal(runtime.Device.Id);
            }

            // 启动主调度循环
            _schedulerTask = RunSchedulerLoopAsync(_cts.Token);

            _logger.LogInformation("设备调度器启动完成");
            await Task.CompletedTask;
        }

        /// <summary>
        /// 停止调度器
        /// </summary>
        public async Task StopAsync()
        {
            if (!_isRunning)
            {
                _logger.LogWarning("调度器未在运行");
                return;
            }

            _logger.LogInformation("正在停止设备调度器...");

            // 停止所有设备
            foreach (var deviceId in _deviceWorkers.Keys.ToList())
            {
                StopDeviceInternal(deviceId);
            }

            // 取消调度循环
            _cts?.Cancel();

            // 等待调度任务完成
            if (_schedulerTask != null)
            {
                try
                {
                    await _schedulerTask;
                }
                catch (OperationCanceledException)
                {
                    // 忽略取消异常
                }
            }

            _isRunning = false;
            _logger.LogInformation("设备调度器已停止");
        }

        /// <summary>
        /// 启动指定设备
        /// </summary>
        /// <param name="deviceId">设备ID</param>
        /// <returns>启动成功返回true，否则返回false</returns>
        public Task<bool> StartDeviceAsync(int deviceId)
        {
            if (!_isRunning)
            {
                _logger.LogWarning("调度器未运行，无法启动设备 {DeviceId}", deviceId);
                return Task.FromResult(false);
            }

            var result = StartDeviceInternal(deviceId);
            return Task.FromResult(result);
        }

        /// <summary>
        /// 停止指定设备
        /// </summary>
        /// <param name="deviceId">设备ID</param>
        /// <returns>停止成功返回true，否则返回false</returns>
        public Task<bool> StopDeviceAsync(int deviceId)
        {
            var result = StopDeviceInternal(deviceId);
            return Task.FromResult(result);
        }

        /// <summary>
        /// 重启指定设备
        /// </summary>
        /// <param name="deviceId">设备ID</param>
        /// <returns>重启成功返回true，否则返回false</returns>
        public async Task<bool> RestartDeviceAsync(int deviceId)
        {
            _logger.LogInformation("正在重启设备 {DeviceId}...", deviceId);

            StopDeviceInternal(deviceId);

            // 等待一小段时间确保设备完全停止
            await Task.Delay(100);

            if (!_isRunning)
            {
                _logger.LogWarning("调度器未运行，无法重启设备 {DeviceId}", deviceId);
                return false;
            }

            var result = StartDeviceInternal(deviceId);

            if (result)
            {
                _logger.LogInformation("设备 {DeviceId} 重启成功", deviceId);
            }

            return result;
        }

        /// <summary>
        /// 检查设备Worker状态
        /// </summary>
        /// <param name="deviceId">设备ID</param>
        /// <returns>设备Worker状态信息，不存在则返回null</returns>
        public DeviceWorkerInfo? GetWorkerStatus(int deviceId)
        {
            return _deviceWorkers.TryGetValue(deviceId, out var info) ? info : null;
        }

        /// <summary>
        /// 获取所有设备Worker状态
        /// </summary>
        /// <returns>设备Worker状态列表</returns>
        public IEnumerable<DeviceWorkerInfo> GetAllWorkerStatus()
        {
            return _deviceWorkers.Values;
        }

        /// <summary>
        /// 添加设备到调度器
        /// </summary>
        /// <param name="deviceId">设备ID</param>
        /// <returns>添加成功返回true，否则返回false</returns>
        public Task<bool> AddDeviceAsync(int deviceId)
        {
            var runtime = _runtimeManager.GetDevice(deviceId);
            if (runtime == null)
            {
                _logger.LogWarning("设备 {DeviceId} 不存在", deviceId);
                return Task.FromResult(false);
            }

            if (_deviceWorkers.ContainsKey(deviceId))
            {
                _logger.LogWarning("设备 {DeviceId} 已在调度器中", deviceId);
                return Task.FromResult(false);
            }

            // 如果调度器正在运行，立即启动设备
            if (_isRunning)
            {
                StartDeviceInternal(deviceId);
            }

            _logger.LogInformation("设备 {DeviceId} 已添加到调度器", deviceId);
            return Task.FromResult(true);
        }

        /// <summary>
        /// 从调度器删除设备
        /// </summary>
        /// <param name="deviceId">设备ID</param>
        /// <returns>删除成功返回true，否则返回false</returns>
        public Task<bool> RemoveDeviceAsync(int deviceId)
        {
            if (!_deviceWorkers.TryRemove(deviceId, out _))
            {
                _logger.LogWarning("设备 {DeviceId} 不在调度器中", deviceId);
                return Task.FromResult(false);
            }

            // 停止设备
            StopDeviceInternal(deviceId);

            _logger.LogInformation("设备 {DeviceId} 已从调度器删除", deviceId);
            return Task.FromResult(true);
        }

        /// <summary>
        /// 内部启动设备
        /// </summary>
        private bool StartDeviceInternal(int deviceId)
        {
            var runtime = _runtimeManager.GetDevice(deviceId);
            if (runtime == null)
            {
                _logger.LogWarning("设备 {DeviceId} 不存在，无法启动", deviceId);
                return false;
            }

            if (_deviceWorkers.ContainsKey(deviceId))
            {
                _logger.LogWarning("设备 {DeviceId} 已在运行中", deviceId);
                return false;
            }

            if (!runtime.Device.IsEnabled)
            {
                _logger.LogWarning("设备 {DeviceId} 未启用，跳过启动", deviceId);
                return false;
            }

            var cts = CancellationTokenSource.CreateLinkedTokenSource(_cts?.Token ?? CancellationToken.None);
            var workerInfo = new DeviceWorkerInfo
            {
                DeviceId = deviceId,
                DeviceKey = runtime.Device.Key,
                Status = WorkerStatus.Starting,
                StartTime = DateTime.Now,
                CancellationTokenSource = cts
            };

            _deviceWorkers[deviceId] = workerInfo;

            // 启动设备工作线程
            _ = Task.Run(async () =>
            {
                try
                {
                    workerInfo.Status = WorkerStatus.Running;
                    runtime.IsRunning = true;
                    runtime.CancellationTokenSource = cts;

                    var worker = new DeviceWorker(runtime, _workerLogger);
                    await worker.WorkerAsync(cts.Token);
                }
                catch (OperationCanceledException)
                {
                    _logger.LogInformation("设备 {DeviceId} 工作线程被取消", deviceId);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "设备 {DeviceId} 工作线程异常", deviceId);
                    workerInfo.Status = WorkerStatus.Error;
                }
                finally
                {
                    runtime.IsRunning = false;
                    workerInfo.Status = WorkerStatus.Stopped;
                    workerInfo.StopTime = DateTime.Now;
                }
            }, cts.Token);

            _logger.LogInformation("设备 {DeviceId} 已启动", deviceId);
            return true;
        }

        /// <summary>
        /// 内部停止设备
        /// </summary>
        private bool StopDeviceInternal(int deviceId)
        {
            if (!_deviceWorkers.TryGetValue(deviceId, out var workerInfo))
            {
                return false;
            }

            try
            {
                workerInfo.Status = WorkerStatus.Stopping;
                workerInfo.CancellationTokenSource?.Cancel();

                var runtime = _runtimeManager.GetDevice(deviceId);
                if (runtime != null)
                {
                    runtime.IsRunning = false;
                    runtime.ConnectionState = DeviceConnectionState.Disconnected;
                }

                _logger.LogInformation("设备 {DeviceId} 已停止", deviceId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "停止设备 {DeviceId} 失败", deviceId);
                return false;
            }
        }

        /// <summary>
        /// 主调度循环
        /// </summary>
        private async Task RunSchedulerLoopAsync(CancellationToken token)
        {
            _logger.LogInformation("调度器主循环已启动");

            while (!token.IsCancellationRequested)
            {
                try
                {
                    // 检查所有设备运行状态，自动重启异常停止的设备
                    foreach (var runtime in _runtimeManager.DeviceRuntimes.Values)
                    {
                        if (runtime.Device.IsEnabled && !runtime.IsRunning && !_deviceWorkers.ContainsKey(runtime.Device.Id))
                        {
                            _logger.LogInformation("检测到设备 {DeviceId} 未运行，尝试自动启动", runtime.Device.Id);
                            StartDeviceInternal(runtime.Device.Id);
                        }
                    }

                    // 调度器 tick 间隔
                    await Task.Delay(1000, token);
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "调度器主循环异常");
                }
            }

            _logger.LogInformation("调度器主循环已停止");
        }
    }

    /// <summary>
    /// 设备Worker状态信息
    /// </summary>
    public class DeviceWorkerInfo
    {
        /// <summary>
        /// 设备ID
        /// </summary>
        public int DeviceId { get; set; }

        /// <summary>
        /// 设备Key
        /// </summary>
        public string DeviceKey { get; set; } = string.Empty;

        /// <summary>
        /// Worker状态
        /// </summary>
        public WorkerStatus Status { get; set; }

        /// <summary>
        /// 启动时间
        /// </summary>
        public DateTime StartTime { get; set; }

        /// <summary>
        /// 停止时间
        /// </summary>
        public DateTime? StopTime { get; set; }

        /// <summary>
        /// 取消令牌源
        /// </summary>
        public CancellationTokenSource? CancellationTokenSource { get; set; }
    }

    /// <summary>
    /// Worker状态枚举
    /// </summary>
    public enum WorkerStatus
    {
        /// <summary>
        /// 启动中
        /// </summary>
        Starting,

        /// <summary>
        /// 运行中
        /// </summary>
        Running,

        /// <summary>
        /// 停止中
        /// </summary>
        Stopping,

        /// <summary>
        /// 已停止
        /// </summary>
        Stopped,

        /// <summary>
        /// 错误
        /// </summary>
        Error
    }
}
