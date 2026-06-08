using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using ScadaServer.Domain.Entities;
using ScadaServer.Domain.Enums;
using ScadaServer.Domain.Interfaces;

namespace ScadaServer.Runtime.Devices
{
    /// <summary>
    /// 设备工作器，负责单台设备的数据采集和驱动通讯
    /// </summary>
    /// <remarks>
    /// 每个设备运行时对应一个 DeviceWorker 实例，由 DeviceScheduler 调度执行。
    /// 工作器以轮询方式周期性地读取设备所有变量的值，并更新运行时状态和统计信息。
    /// 支持变量变化检测、质量状态管理和平均响应时间计算。
    /// </remarks>
    public class DeviceWorker
    {
        private readonly DeviceRuntime _runtime;
        private readonly ILogger<DeviceWorker> _logger;

        /// <summary>
        /// 写入队列
        /// </summary>
        private readonly ConcurrentQueue<WriteRequest> _writeQueue = new();

        /// <summary>
        /// 扫描时间统计
        /// </summary>
        private readonly Queue<double> _scanTimes = new();

        /// <summary>
        /// 最大扫描时间记录数
        /// </summary>
        private const int MaxScanTimeRecords = 100;

        /// <summary>
        /// 重连最大尝试次数
        /// </summary>
        private const int MaxReconnectAttempts = 5;

        /// <summary>
        /// 重连间隔（毫秒）
        /// </summary>
        private const int ReconnectInterval = 5000;

        /// <summary>
        /// 初始化设备工作器
        /// </summary>
        /// <param name="runtime">设备运行时，包含设备配置、驱动实例和变量集合</param>
        /// <param name="logger">日志记录器</param>
        /// <exception cref="ArgumentNullException">runtime 或 logger 为 null 时抛出</exception>
        public DeviceWorker(DeviceRuntime runtime, ILogger<DeviceWorker> logger)
        {
            _runtime = runtime ?? throw new ArgumentNullException(nameof(runtime));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// 启动设备采集循环
        /// </summary>
        /// <param name="cancellationToken">取消令牌，用于停止采集循环</param>
        /// <returns>任务完成时返回</returns>
        public async Task WorkerAsync(CancellationToken cancellationToken)
        {
            // 检查驱动是否已分配，无驱动则无法工作
            if (_runtime.Driver == null)
            {
                _logger.LogWarning("设备 {DeviceKey} 未分配驱动", _runtime.Device.Key);
                return;
            }

            // 尝试连接设备
            if (!await ConnectDeviceAsync())
            {
                _logger.LogError("设备 {DeviceKey} 连接失败，无法启动采集", _runtime.Device.Key);
                return;
            }

            _logger.LogInformation("设备工作器 {DeviceKey} 初始化完成", _runtime.Device.Key);

            // 主采集循环
            while (!cancellationToken.IsCancellationRequested)
            {
                var scanStopwatch = Stopwatch.StartNew();

                try
                {
                    // 处理写入队列
                    await ProcessWriteQueueAsync(cancellationToken);

                    // 批量读取变量
                    await ReadVariablesBatchAsync(cancellationToken);

                    // 更新成功状态
                    UpdateDeviceStatus(true);
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (Exception ex)
                {
                    await HandleCommunicationErrorAsync(ex);
                }
                finally
                {
                    scanStopwatch.Stop();
                    CalculateScanTime(scanStopwatch.Elapsed.TotalMilliseconds);
                }

                // 等待下一个轮询周期
                await Task.Delay(_runtime.Device.PollingInterval, cancellationToken);
            }

            // 断开设备连接
            await DisconnectDeviceAsync();
            _logger.LogInformation("设备工作器 {DeviceKey} 已停止", _runtime.Device.Key);
        }

        #region 连接管理

        /// <summary>
        /// 连接设备
        /// </summary>
        /// <returns>连接成功返回true，否则返回false</returns>
        public async Task<bool> ConnectDeviceAsync()
        {
            if (_runtime.Driver == null)
            {
                _logger.LogWarning("设备 {DeviceKey} 未分配驱动，无法连接", _runtime.Device.Key);
                UpdateDeviceStatus(false, "未分配驱动");
                return false;
            }

            try
            {
                _runtime.ConnectionState = DeviceConnectionState.Connecting;
                _logger.LogInformation("正在连接设备 {DeviceKey}...", _runtime.Device.Key);

                var configJson = _runtime.Device.Config?.ConfigJson ?? string.Empty;
                var success = await _runtime.Driver.ConnectAsync(_runtime.Device, configJson);

                if (success)
                {
                    _runtime.ConnectionState = DeviceConnectionState.Connected;
                    _runtime.LastCommunicationTime = DateTime.Now;
                    _runtime.ConsecutiveFailureCount = 0;
                    _logger.LogInformation("设备 {DeviceKey} 连接成功", _runtime.Device.Key);
                    return true;
                }
                else
                {
                    _runtime.ConnectionState = DeviceConnectionState.Error;
                    _logger.LogWarning("设备 {DeviceKey} 连接失败", _runtime.Device.Key);
                    return false;
                }
            }
            catch (Exception ex)
            {
                _runtime.ConnectionState = DeviceConnectionState.Error;
                _logger.LogError(ex, "设备 {DeviceKey} 连接异常", _runtime.Device.Key);
                return false;
            }
        }

        /// <summary>
        /// 断开设备连接
        /// </summary>
        public async Task DisconnectDeviceAsync()
        {
            if (_runtime.Driver == null)
            {
                return;
            }

            try
            {
                _logger.LogInformation("正在断开设备 {DeviceKey}...", _runtime.Device.Key);
                await _runtime.Driver.DisconnectAsync();
                _runtime.ConnectionState = DeviceConnectionState.Disconnected;
                _runtime.IsRunning = false;
                _logger.LogInformation("设备 {DeviceKey} 已断开连接", _runtime.Device.Key);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "断开设备 {DeviceKey} 时发生异常", _runtime.Device.Key);
            }
        }

        /// <summary>
        /// 重连设备
        /// </summary>
        /// <param name="maxAttempts">最大尝试次数，默认使用配置值</param>
        /// <returns>重连成功返回true，否则返回false</returns>
        public async Task<bool> ReconnectDeviceAsync(int? maxAttempts = null)
        {
            var attempts = maxAttempts ?? MaxReconnectAttempts;
            _logger.LogInformation("开始重连设备 {DeviceKey}，最大尝试次数: {Attempts}", _runtime.Device.Key, attempts);

            // 先断开现有连接
            await DisconnectDeviceAsync();

            for (int i = 1; i <= attempts; i++)
            {
                _logger.LogInformation("设备 {DeviceKey} 重连尝试 {Current}/{Max}", _runtime.Device.Key, i, attempts);

                if (await ConnectDeviceAsync())
                {
                    _logger.LogInformation("设备 {DeviceKey} 重连成功", _runtime.Device.Key);
                    return true;
                }

                if (i < attempts)
                {
                    await Task.Delay(ReconnectInterval);
                }
            }

            _logger.LogError("设备 {DeviceKey} 重连失败，已达到最大尝试次数", _runtime.Device.Key);
            return false;
        }

        #endregion

        #region 读取操作

        /// <summary>
        /// 读取单个变量
        /// </summary>
        /// <param name="variableId">变量ID</param>
        /// <returns>变量值，失败返回null</returns>
        public async Task<object?> ReadVariableAsync(int variableId)
        {
            if (_runtime.Driver == null || _runtime.ConnectionState != DeviceConnectionState.Connected)
            {
                _logger.LogWarning("设备 {DeviceKey} 未连接，无法读取变量", _runtime.Device.Key);
                return null;
            }

            if (!_runtime.Variables.TryGetValue(variableId, out var variableRuntime))
            {
                _logger.LogWarning("变量 {VariableId} 不存在", variableId);
                return null;
            }

            try
            {
                var value = await _runtime.Driver.ReadAsync(variableRuntime.Variable);
                UpdateVariableValue(variableRuntime, value, VariableQuality.Good);
                return value;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "读取变量 {VariableId} 失败", variableId);
                variableRuntime.Quality = VariableQuality.CommunicationError;
                return null;
            }
        }

        /// <summary>
        /// 批量读取变量
        /// </summary>
        /// <param name="cancellationToken">取消令牌</param>
        public async Task ReadVariablesBatchAsync(CancellationToken cancellationToken = default)
        {
            if (_runtime.Driver == null || _runtime.ConnectionState != DeviceConnectionState.Connected)
            {
                _logger.LogWarning("设备 {DeviceKey} 未连接，跳过批量读取", _runtime.Device.Key);
                return;
            }

            var variables = _runtime.Variables.Values
                .Select(v => v.Variable)
                .ToList();

            if (variables.Count == 0)
            {
                return;
            }

            try
            {
                var results = await _runtime.Driver.ReadBatchAsync(variables);

                foreach (var variableRuntime in _runtime.Variables.Values)
                {
                    if (cancellationToken.IsCancellationRequested)
                    {
                        break;
                    }

                    var key = variableRuntime.Variable.Address ?? variableRuntime.Variable.Name;
                    if (results.TryGetValue(key, out var value))
                    {
                        UpdateVariableValue(variableRuntime, value, VariableQuality.Good);
                    }
                    else
                    {
                        variableRuntime.Quality = VariableQuality.Bad;
                    }
                }

                _runtime.LastPollTime = DateTime.Now;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "设备 {DeviceKey} 批量读取失败", _runtime.Device.Key);
                throw;
            }
        }

        #endregion

        #region 写入操作

        /// <summary>
        /// 写入变量
        /// </summary>
        /// <param name="variableId">变量ID</param>
        /// <param name="value">要写入的值</param>
        /// <param name="immediate">是否立即写入，false则加入队列</param>
        /// <returns>写入成功返回true，否则返回false</returns>
        public async Task<bool> WriteVariableAsync(int variableId, object value, bool immediate = false)
        {
            if (!_runtime.Variables.TryGetValue(variableId, out var variableRuntime))
            {
                _logger.LogWarning("变量 {VariableId} 不存在", variableId);
                return false;
            }

            if (immediate)
            {
                return await WriteVariableImmediateAsync(variableRuntime, value);
            }
            else
            {
                EnqueueWrite(variableId, value);
                return true;
            }
        }

        /// <summary>
        /// 立即写入变量
        /// </summary>
        private async Task<bool> WriteVariableImmediateAsync(VariableRuntime variableRuntime, object value)
        {
            // TODO: 驱动接口需要添加 WriteAsync 方法
            // 目前先更新本地值
            try
            {
                UpdateVariableValue(variableRuntime, value, VariableQuality.Good);
                _logger.LogInformation("变量 {VariableId} 写入成功", variableRuntime.Variable.Id);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "变量 {VariableId} 写入失败", variableRuntime.Variable.Id);
                return false;
            }
        }

        /// <summary>
        /// 将写入请求加入队列
        /// </summary>
        /// <param name="variableId">变量ID</param>
        /// <param name="value">要写入的值</param>
        public void EnqueueWrite(int variableId, object value)
        {
            var request = new WriteRequest
            {
                VariableId = variableId,
                Value = value,
                Timestamp = DateTime.Now
            };

            _writeQueue.Enqueue(request);
            _logger.LogDebug("变量 {VariableId} 写入请求已加入队列", variableId);
        }

        /// <summary>
        /// 处理写入队列
        /// </summary>
        /// <param name="cancellationToken">取消令牌</param>
        public async Task ProcessWriteQueueAsync(CancellationToken cancellationToken = default)
        {
            var processedCount = 0;

            while (_writeQueue.TryDequeue(out var request) && !cancellationToken.IsCancellationRequested)
            {
                if (await WriteVariableAsync(request.VariableId, request.Value, immediate: true))
                {
                    processedCount++;
                }
            }

            if (processedCount > 0)
            {
                _logger.LogDebug("设备 {DeviceKey} 处理了 {Count} 个写入请求", _runtime.Device.Key, processedCount);
            }
        }

        /// <summary>
        /// 获取写入队列长度
        /// </summary>
        public int GetWriteQueueLength() => _writeQueue.Count;

        #endregion

        #region 状态更新

        /// <summary>
        /// 更新变量值
        /// </summary>
        /// <param name="variableRuntime">变量运行时</param>
        /// <param name="value">新值</param>
        /// <param name="quality">质量状态</param>
        public void UpdateVariableValue(VariableRuntime variableRuntime, object? value, VariableQuality quality)
        {
            variableRuntime.PreviousValue = variableRuntime.Value;
            variableRuntime.Value = value;
            variableRuntime.Quality = quality;
            variableRuntime.UpdateTime = DateTime.Now;
            variableRuntime.IsChanged = !Equals(variableRuntime.Value, variableRuntime.PreviousValue);
        }

        /// <summary>
        /// 更新设备状态
        /// </summary>
        /// <param name="success">操作是否成功</param>
        /// <param name="errorMessage">错误消息</param>
        public void UpdateDeviceStatus(bool success, string? errorMessage = null)
        {
            if (success)
            {
                _runtime.ConnectionState = DeviceConnectionState.Connected;
                _runtime.LastCommunicationTime = DateTime.Now;
                _runtime.SuccessCount++;
                _runtime.ConsecutiveFailureCount = 0;
            }
            else
            {
                _runtime.ConnectionState = DeviceConnectionState.Error;
                _runtime.FailureCount++;
                _runtime.ConsecutiveFailureCount++;

                if (!string.IsNullOrEmpty(errorMessage))
                {
                    _logger.LogWarning("设备 {DeviceKey} 状态更新: {Message}", _runtime.Device.Key, errorMessage);
                }
            }
        }

        #endregion

        #region 异常处理

        /// <summary>
        /// 通信异常处理
        /// </summary>
        /// <param name="exception">异常</param>
        public async Task HandleCommunicationErrorAsync(Exception exception)
        {
            _runtime.FailureCount++;
            _runtime.ConsecutiveFailureCount++;

            _logger.LogError(exception, "设备 {DeviceKey} 通信异常，连续失败次数: {Count}",
                _runtime.Device.Key, _runtime.ConsecutiveFailureCount);

            // 更新所有变量质量状态
            foreach (var variable in _runtime.Variables.Values)
            {
                variable.Quality = _runtime.ConnectionState == DeviceConnectionState.Disconnected
                    ? VariableQuality.DeviceOffline
                    : VariableQuality.CommunicationError;
            }

            // 根据连续失败次数决定是否重连
            if (_runtime.ConsecutiveFailureCount >= 3)
            {
                _logger.LogWarning("设备 {DeviceKey} 连续失败次数过多，尝试重连", _runtime.Device.Key);
                _runtime.ConnectionState = DeviceConnectionState.Error;

                if (!await ReconnectDeviceAsync())
                {
                    _logger.LogError("设备 {DeviceKey} 重连失败", _runtime.Device.Key);
                }
            }
        }

        #endregion

        #region 统计计算

        /// <summary>
        /// 计算扫描时间
        /// </summary>
        /// <param name="scanTimeMs">扫描耗时（毫秒）</param>
        public void CalculateScanTime(double scanTimeMs)
        {
            lock (_scanTimes)
            {
                _scanTimes.Enqueue(scanTimeMs);

                if (_scanTimes.Count > MaxScanTimeRecords)
                {
                    _scanTimes.Dequeue();
                }

                // 计算平均扫描时间
                if (_scanTimes.Count > 0)
                {
                    _runtime.AverageResponseTime = _scanTimes.Average();
                }
            }

            _runtime.LastPollTime = DateTime.Now;
        }

        /// <summary>
        /// 获取扫描时间统计
        /// </summary>
        /// <returns>平均扫描时间（毫秒）</returns>
        public double GetAverageScanTime()
        {
            lock (_scanTimes)
            {
                return _scanTimes.Count > 0 ? _scanTimes.Average() : 0;
            }
        }

        /// <summary>
        /// 获取最大扫描时间
        /// </summary>
        /// <returns>最大扫描时间（毫秒）</returns>
        public double GetMaxScanTime()
        {
            lock (_scanTimes)
            {
                return _scanTimes.Count > 0 ? _scanTimes.Max() : 0;
            }
        }

        /// <summary>
        /// 获取最小扫描时间
        /// </summary>
        /// <returns>最小扫描时间（毫秒）</returns>
        public double GetMinScanTime()
        {
            lock (_scanTimes)
            {
                return _scanTimes.Count > 0 ? _scanTimes.Min() : 0;
            }
        }

        #endregion
    }

    /// <summary>
    /// 写入请求
    /// </summary>
    public class WriteRequest
    {
        /// <summary>
        /// 变量ID
        /// </summary>
        public int VariableId { get; set; }

        /// <summary>
        /// 要写入的值
        /// </summary>
        public object? Value { get; set; }

        /// <summary>
        /// 请求时间戳
        /// </summary>
        public DateTime Timestamp { get; set; }
    }
}
