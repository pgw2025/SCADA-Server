using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using ScadaServer.Domain.Entities;
using ScadaServer.Domain.Interfaces.Repositories;
using ScadaServer.Runtime.Devices;
using ScadaServer.Runtime.Interface;
using ScadaServer.Runtime.Variables;

namespace ScadaServer.Runtime
{
    /// <summary>
    /// SCADA运行时管理器，负责管理所有设备运行时的生命周期
    /// </summary>
    public class RuntimeManager : IRuntimeManager
    {
        #region 字典和映射

        /// <summary>
        /// 设备运行时字典，键为设备ID
        /// </summary>
        public ConcurrentDictionary<int, DeviceRuntime> DeviceRuntimes { get; } = new();

        /// <summary>
        /// 设备Key到ID的映射
        /// </summary>
        private readonly ConcurrentDictionary<string, int> _deviceKeyMap = new();

        /// <summary>
        /// 区域运行时字典，键为区域ID
        /// </summary>
        public ConcurrentDictionary<int, AreaRuntime> AreaRuntimes { get; } = new();

        /// <summary>
        /// 数据模型运行时字典，键为模型ID
        /// </summary>
        public ConcurrentDictionary<int, DataModelRuntime> DataModelRuntimes { get; } = new();

        /// <summary>
        /// 变量Key到变量ID的映射（全局）
        /// </summary>
        private readonly ConcurrentDictionary<string, int> _variableKeyMap = new();

        #endregion

        #region 依赖注入

        private readonly ILogger<RuntimeManager> _logger;
        private readonly IDeviceRepository _deviceRepository;
        private readonly IAreaRepository _areaRepository;
        private readonly IDataModelRepository _dataModelRepository;
        private readonly IModelVariableRepository _modelVariableRepository;
        private readonly IDeviceConfigRepository _deviceConfigRepository;

        private DeviceScheduler? _scheduler;
        private CancellationToken _cancellationToken;
        private bool _isRunning;

        /// <summary>
        /// 初始化运行时管理器
        /// </summary>
        /// <param name="logger">日志记录器</param>
        /// <param name="deviceRepository">设备仓储</param>
        /// <param name="areaRepository">区域仓储</param>
        /// <param name="dataModelRepository">数据模型仓储</param>
        /// <param name="modelVariableRepository">模型变量仓储</param>
        /// <param name="deviceConfigRepository">设备配置仓储</param>
        public RuntimeManager(
            ILogger<RuntimeManager> logger,
            IDeviceRepository deviceRepository,
            IAreaRepository areaRepository,
            IDataModelRepository dataModelRepository,
            IModelVariableRepository modelVariableRepository,
            IDeviceConfigRepository deviceConfigRepository)
        {
            _logger = logger;
            _deviceRepository = deviceRepository;
            _areaRepository = areaRepository;
            _dataModelRepository = dataModelRepository;
            _modelVariableRepository = modelVariableRepository;
            _deviceConfigRepository = deviceConfigRepository;
        }

        #endregion

        #region 初始化

        /// <inheritdoc/>
        public async Task InitializeAsync()
        {
            _logger.LogInformation("正在初始化运行时管理器...");

            // 清空现有数据
            ClearAllData();

            // 加载基础数据
            await LoadAreasAsync();
            await LoadDataModelsAsync();
            await LoadModelVariablesAsync();
            await LoadDevicesAsync();

            _logger.LogInformation("运行时管理器初始化完成，加载设备: {DeviceCount}，区域: {AreaCount}，模型: {ModelCount}，变量: {VariableCount}",
                DeviceRuntimes.Count, AreaRuntimes.Count, DataModelRuntimes.Count, DataModelRuntimes.Values.Sum(m => m.TotalVariableCount));
        }

        /// <summary>
        /// 清空所有数据
        /// </summary>
        private void ClearAllData()
        {
            DeviceRuntimes.Clear();
            _deviceKeyMap.Clear();
            Areas.Clear();
            DataModels.Clear();
            ModelVariables.Clear();
            _variableKeyMap.Clear();
        }

        /// <summary>
        /// 加载区域数据
        /// </summary>
        private async Task LoadAreasAsync()
        {
            _logger.LogInformation("正在加载区域数据...");

            var areas = await _areaRepository.GetListAsync();
            foreach (var area in areas)
            {
                Areas[area.Id] = area;
            }

            _logger.LogInformation("区域数据加载完成，共 {Count} 个区域", areas.Count);
        }

        /// <summary>
        /// 加载数据模型数据
        /// </summary>
        private async Task LoadDataModelsAsync()
        {
            _logger.LogInformation("正在加载数据模型数据...");

            var models = await _dataModelRepository.GetListAsync();
            foreach (var model in models)
            {
                DataModels[model.Id] = model;
            }

            _logger.LogInformation("数据模型加载完成，共 {Count} 个模型", models.Count);
        }

        /// <summary>
        /// 加载模型变量数据
        /// </summary>
        private async Task LoadModelVariablesAsync()
        {
            _logger.LogInformation("正在加载模型变量数据...");

            var variables = await _modelVariableRepository.GetListAsync();

            // 按模型ID分组并添加到对应的 DataModelRuntime
            var groupedVariables = variables.GroupBy(v => v.ModelId);
            foreach (var group in groupedVariables)
            {
                if (DataModelRuntimes.TryGetValue(group.Key, out var modelRuntime))
                {
                    foreach (var variable in group)
                    {
                        modelRuntime.AddVariable(variable);

                        // 建立变量Key映射
                        if (!string.IsNullOrEmpty(variable.Key))
                        {
                            _variableKeyMap[variable.Key] = variable.Id;
                        }
                    }
                }
            }

            _logger.LogInformation("模型变量加载完成，共 {Count} 个变量", variables.Count);
        }

        /// <summary>
        /// 加载设备数据
        /// </summary>
        private async Task LoadDevicesAsync()
        {
            _logger.LogInformation("正在加载设备数据...");

            var devices = await _deviceRepository.GetListAsync();

            // 加载设备配置
            var deviceConfigs = await _deviceConfigRepository.GetListAsync();
            var configDict = deviceConfigs.ToDictionary(c => c.DeviceId);

            foreach (var device in devices)
            {
                // 关联配置
                if (configDict.TryGetValue(device.Id, out var config))
                {
                    device.Config = config;
                }

                // 创建设备运行时
                var runtime = CreateDeviceRuntime(device);
                DeviceRuntimes[device.Id] = runtime;
                _deviceKeyMap[device.Key] = device.Id;
            }

            _logger.LogInformation("设备数据加载完成，共 {Count} 台设备", devices.Count);
        }

        /// <summary>
        /// 创建设备运行时
        /// </summary>
        /// <param name="device">设备实体</param>
        /// <returns>设备运行时</returns>
        private DeviceRuntime CreateDeviceRuntime(Device device)
        {
            var runtime = new DeviceRuntime(device);

            // 关联区域
            if (Areas.TryGetValue(device.AreaId, out var area))
            {
                // runtime.Area = area;
            }

            // 关联模型
            if (DataModels.TryGetValue(device.ModelId, out var model))
            {
                // runtime.Model = model;

                // 加载模型变量到设备运行时
                if (ModelVariables.TryGetValue(device.ModelId, out var variables))
                {
                    foreach (var variable in variables)
                    {
                        var variableRuntime = new VariableRuntime
                        {
                            Variable = variable,
                            Quality = Domain.Enums.VariableQuality.Initializing
                        };
                        runtime.Variables[variable.Id] = variableRuntime;
                    }
                }
            }

            return runtime;
        }

        #endregion

        #region 启动停止

        /// <inheritdoc/>
        public async Task StartAsync(CancellationToken token, int maxConcurrentWorkers = 10)
        {
            if (_isRunning)
            {
                _logger.LogWarning("运行时管理器已在运行中");
                return;
            }

            _cancellationToken = token;
            _logger.LogInformation("正在启动运行时管理器，最大并发数: {MaxConcurrentWorkers}", maxConcurrentWorkers);

            // _scheduler = new DeviceScheduler(this, maxConcurrentWorkers, _logger);
            // await _scheduler.StartAsync(token);

            _isRunning = true;
            _logger.LogInformation("运行时管理器启动完成");
            await Task.CompletedTask;
        }

        /// <inheritdoc/>
        public async Task StopAsync()
        {
            if (!_isRunning)
            {
                _logger.LogWarning("运行时管理器未在运行");
                return;
            }

            _logger.LogInformation("正在停止运行时管理器...");

            // 停止所有设备运行时
            foreach (var runtime in DeviceRuntimes.Values)
            {
                try
                {
                    runtime.CancellationTokenSource?.Cancel();
                    runtime.IsRunning = false;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "停止设备 {DeviceId} 运行时失败", runtime.Device.Id);
                }
            }

            // 停止调度器
            if (_scheduler != null)
            {
                // await _scheduler.StopAsync();
            }

            _isRunning = false;
            _logger.LogInformation("运行时管理器已停止");
            await Task.CompletedTask;
        }

        /// <inheritdoc/>
        public async Task ReloadConfigAsync()
        {
            _logger.LogInformation("正在重载配置...");

            // 停止当前运行
            await StopAsync();

            // 重新初始化
            await InitializeAsync();

            // 重新启动
            if (_cancellationToken.CanBeCanceled)
            {
                await StartAsync(_cancellationToken);
            }

            _logger.LogInformation("配置重载完成");
        }

        #endregion

        #region 设备管理

        /// <inheritdoc/>
        public IEnumerable<DeviceRuntime> GetDevices()
        {
            return DeviceRuntimes.Values;
        }

        /// <inheritdoc/>
        public DeviceRuntime? GetDevice(int deviceId)
        {
            return DeviceRuntimes.TryGetValue(deviceId, out var runtime) ? runtime : null;
        }

        /// <inheritdoc/>
        public DeviceRuntime? GetDevice(string deviceKey)
        {
            if (_deviceKeyMap.TryGetValue(deviceKey, out var deviceId))
            {
                return GetDevice(deviceId);
            }
            return null;
        }

        /// <inheritdoc/>
        public IEnumerable<VariableRuntime> GetVariables(int deviceId)
        {
            var runtime = GetDevice(deviceId);
            return runtime?.Variables.Values ?? Enumerable.Empty<VariableRuntime>();
        }

        /// <inheritdoc/>
        public VariableRuntime? GetVariable(int deviceId, int variableId)
        {
            var runtime = GetDevice(deviceId);
            if (runtime != null && runtime.Variables.TryGetValue(variableId, out var variable))
            {
                return variable;
            }
            return null;
        }

        /// <inheritdoc/>
        public Task<bool> AddDeviceAsync(Device device)
        {
            try
            {
                if (DeviceRuntimes.ContainsKey(device.Id))
                {
                    _logger.LogWarning("设备 {DeviceId} 已存在", device.Id);
                    return Task.FromResult(false);
                }

                var runtime = CreateDeviceRuntime(device);
                DeviceRuntimes[device.Id] = runtime;
                _deviceKeyMap[device.Key] = device.Id;

                _logger.LogInformation("设备 {DeviceId} ({DeviceName}) 添加成功", device.Id, device.Name);
                return Task.FromResult(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "添加设备 {DeviceId} 失败", device.Id);
                return Task.FromResult(false);
            }
        }

        /// <inheritdoc/>
        public Task<bool> RemoveDeviceAsync(int deviceId)
        {
            try
            {
                if (!DeviceRuntimes.TryRemove(deviceId, out var runtime))
                {
                    _logger.LogWarning("设备 {DeviceId} 不存在", deviceId);
                    return Task.FromResult(false);
                }

                // 停止设备运行
                runtime.CancellationTokenSource?.Cancel();
                runtime.IsRunning = false;

                // 移除Key映射
                _deviceKeyMap.TryRemove(runtime.Device.Key, out _);

                _logger.LogInformation("设备 {DeviceId} 删除成功", deviceId);
                return Task.FromResult(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "删除设备 {DeviceId} 失败", deviceId);
                return Task.FromResult(false);
            }
        }

        /// <inheritdoc/>
        public Task<bool> UpdateDeviceAsync(Device device)
        {
            try
            {
                if (!DeviceRuntimes.TryGetValue(device.Id, out var runtime))
                {
                    _logger.LogWarning("设备 {DeviceId} 不存在", device.Id);
                    return Task.FromResult(false);
                }

                // 如果Key变更，更新映射
                if (runtime.Device.Key != device.Key)
                {
                    _deviceKeyMap.TryRemove(runtime.Device.Key, out _);
                    _deviceKeyMap[device.Key] = device.Id;
                }

                // 更新设备信息需要重新创建运行时
                var newRuntime = CreateDeviceRuntime(device);
                DeviceRuntimes[device.Id] = newRuntime;

                _logger.LogInformation("设备 {DeviceId} ({DeviceName}) 更新成功", device.Id, device.Name);
                return Task.FromResult(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "更新设备 {DeviceId} 失败", device.Id);
                return Task.FromResult(false);
            }
        }

        /// <summary>
        /// 注册设备运行时
        /// </summary>
        /// <param name="runtime">设备运行时</param>
        public void RegisterDevice(DeviceRuntime runtime)
        {
            DeviceRuntimes[runtime.Device.Id] = runtime;
            _deviceKeyMap[runtime.Device.Key] = runtime.Device.Id;
        }

        #endregion

        #region 区域管理

        /// <summary>
        /// 获取所有区域
        /// </summary>
        public IEnumerable<Area> GetAreas()
        {
            return Areas.Values;
        }

        /// <summary>
        /// 根据ID获取区域
        /// </summary>
        public Area? GetArea(int areaId)
        {
            return Areas.TryGetValue(areaId, out var area) ? area : null;
        }

        /// <summary>
        /// 添加区域
        /// </summary>
        public bool AddArea(Area area)
        {
            if (Areas.ContainsKey(area.Id))
            {
                _logger.LogWarning("区域 {AreaId} 已存在", area.Id);
                return false;
            }

            Areas[area.Id] = area;
            _logger.LogInformation("区域 {AreaId} ({AreaName}) 添加成功", area.Id, area.Name);
            return true;
        }

        /// <summary>
        /// 移除区域
        /// </summary>
        public bool RemoveArea(int areaId)
        {
            if (Areas.TryRemove(areaId, out _))
            {
                _logger.LogInformation("区域 {AreaId} 删除成功", areaId);
                return true;
            }
            return false;
        }

        #endregion

        #region 模型管理

        /// <summary>
        /// 获取所有数据模型运行时
        /// </summary>
        public IEnumerable<DataModelRuntime> GetDataModelRuntimes()
        {
            return DataModelRuntimes.Values;
        }

        /// <summary>
        /// 根据ID获取数据模型运行时
        /// </summary>
        public DataModelRuntime? GetDataModelRuntime(int modelId)
        {
            return DataModelRuntimes.TryGetValue(modelId, out var runtime) ? runtime : null;
        }

        /// <summary>
        /// 获取模型下的所有变量运行时
        /// </summary>
        public IEnumerable<ModelVariableRuntime> GetModelVariableRuntimes(int modelId)
        {
            var runtime = GetDataModelRuntime(modelId);
            return runtime?.VariableRuntimes.Values ?? Enumerable.Empty<ModelVariableRuntime>();
        }

        /// <summary>
        /// 根据Key获取变量运行时
        /// </summary>
        public ModelVariableRuntime? GetVariableRuntimeByKey(string variableKey)
        {
            if (_variableKeyMap.TryGetValue(variableKey, out var variableId))
            {
                foreach (var modelRuntime in DataModelRuntimes.Values)
                {
                    var variableRuntime = modelRuntime.GetVariableRuntime(variableId);
                    if (variableRuntime != null)
                    {
                        return variableRuntime;
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// 添加数据模型
        /// </summary>
        public bool AddDataModel(DataModel model)
        {
            if (DataModelRuntimes.ContainsKey(model.Id))
            {
                _logger.LogWarning("数据模型 {ModelId} 已存在", model.Id);
                return false;
            }

            var runtime = new DataModelRuntime(model);
            DataModelRuntimes[model.Id] = runtime;
            _logger.LogInformation("数据模型 {ModelId} ({ModelName}) 添加成功", model.Id, model.Name);
            return true;
        }

        /// <summary>
        /// 移除数据模型
        /// </summary>
        public bool RemoveDataModel(int modelId)
        {
            if (DataModelRuntimes.TryRemove(modelId, out _))
            {
                _logger.LogInformation("数据模型 {ModelId} 删除成功", modelId);
                return true;
            }
            return false;
        }

        #endregion
    }
}
