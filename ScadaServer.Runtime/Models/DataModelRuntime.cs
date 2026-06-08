using ScadaServer.Domain.Entities;
using ScadaServer.Domain.Enums;
using System.Collections.Concurrent;
using System.Threading;

namespace ScadaServer.Runtime.Models;

/// <summary>
/// 数据模型运行时类，管理数据模型级别状态和变量
/// </summary>
public class DataModelRuntime
{
    private readonly DataModel _dataModel;

    /// <summary>
    /// 数据模型实体
    /// </summary>
    public DataModel DataModel { get; init; }

    /// <summary>
    /// 模型变量运行时字典，键为变量ID
    /// </summary>
    public ConcurrentDictionary<int, ModelVariableRuntime> VariableRuntimes { get; } = new();

    /// <summary>
    /// 使用该模型的设备ID列表
    /// </summary>
    public ConcurrentBag<int> DeviceIds { get; } = new();

    /// <summary>
    /// 模型状态
    /// </summary>
    public DataModelStatus Status { get; set; } = DataModelStatus.Normal;

    /// <summary>
    /// 使用该模型的正常设备数量
    /// </summary>
    public int NormalDeviceCount { get; set; }

    /// <summary>
    /// 使用该模型的异常设备数量
    /// </summary>
    public int ErrorDeviceCount { get; set; }

    /// <summary>
    /// 变量总数
    /// </summary>
    public int TotalVariableCount => VariableRuntimes.Count;

    /// <summary>
    /// 正常变量数量
    /// </summary>
    public int GoodVariableCount => VariableRuntimes.Values.Count(v => v.Quality == VariableQuality.Good);

    /// <summary>
    /// 异常变量数量
    /// </summary>
    public int BadVariableCount => VariableRuntimes.Values.Count(v => v.Quality != VariableQuality.Good);

    /// <summary>
    /// 总读取次数
    /// </summary>
    public long TotalReadCount { get; set; }

    /// <summary>
    /// 总写入次数
    /// </summary>
    public long TotalWriteCount { get; set; }

    /// <summary>
    /// 读取失败次数
    /// </summary>
    public long ReadFailureCount { get; set; }

    /// <summary>
    /// 写入失败次数
    /// </summary>
    public long WriteFailureCount { get; set; }

    /// <summary>
    /// 最后更新时间
    /// </summary>
    public DateTime LastUpdateTime { get; set; } = DateTime.Now;

    /// <summary>
    /// 运行时锁
    /// </summary>
    public SemaphoreSlim Lock { get; } = new(1, 1);

    /// <summary>
    /// 取消令牌源
    /// </summary>
    public CancellationTokenSource? CancellationTokenSource { get; set; }

    /// <summary>
    /// 是否正在运行
    /// </summary>
    public bool IsRunning { get; set; }

    /// <summary>
    /// 初始化数据模型运行时
    /// </summary>
    /// <param name="dataModel">数据模型实体</param>
    public DataModelRuntime(DataModel dataModel)
    {
        _dataModel = dataModel;
        DataModel = dataModel;
    }

    /// <summary>
    /// 添加变量到模型
    /// </summary>
    /// <param name="variable">模型变量</param>
    public void AddVariable(ModelVariable variable)
    {
        var runtime = new ModelVariableRuntime(variable);
        VariableRuntimes[variable.Id] = runtime;
    }

    /// <summary>
    /// 添加变量运行时
    /// </summary>
    /// <param name="variableRuntime">变量运行时</param>
    public void AddVariableRuntime(ModelVariableRuntime variableRuntime)
    {
        VariableRuntimes[variableRuntime.Variable.Id] = variableRuntime;
    }

    /// <summary>
    /// 获取变量运行时
    /// </summary>
    /// <param name="variableId">变量ID</param>
    public ModelVariableRuntime? GetVariableRuntime(int variableId)
    {
        return VariableRuntimes.TryGetValue(variableId, out var runtime) ? runtime : null;
    }

    /// <summary>
    /// 获取变量运行时
    /// </summary>
    /// <param name="variableKey">变量Key</param>
    public ModelVariableRuntime? GetVariableRuntime(string variableKey)
    {
        return VariableRuntimes.Values.FirstOrDefault(v => v.Variable.Key == variableKey);
    }

    /// <summary>
    /// 添加设备到模型
    /// </summary>
    /// <param name="deviceId">设备ID</param>
    public void AddDevice(int deviceId)
    {
        DeviceIds.Add(deviceId);
    }

    /// <summary>
    /// 从模型移除设备
    /// </summary>
    /// <param name="deviceId">设备ID</param>
    public bool RemoveDevice(int deviceId)
    {
        return DeviceIds.TryTake(out var id) && id == deviceId;
    }

    /// <summary>
    /// 更新模型状态
    /// </summary>
    public void UpdateStatus()
    {
        // 根据设备状态更新模型状态
        if (DeviceIds.IsEmpty)
        {
            Status = DataModelStatus.NoDevices;
        }
        else if (ErrorDeviceCount == DeviceIds.Count)
        {
            Status = DataModelStatus.Error;
        }
        else if (BadVariableCount > 0 && BadVariableCount < TotalVariableCount)
        {
            Status = DataModelStatus.Warning;
        }
        else if (ReadFailureCount > 0 || WriteFailureCount > 0)
        {
            Status = DataModelStatus.Warning;
        }
        else
        {
            Status = DataModelStatus.Normal;
        }

        LastUpdateTime = DateTime.Now;
    }

    /// <summary>
    /// 记录读取操作
    /// </summary>
    /// <param name="success">是否成功</param>
    public void RecordRead(bool success)
    {
        TotalReadCount++;
        if (!success)
        {
            ReadFailureCount++;
        }
    }

    /// <summary>
    /// 记录写入操作
    /// </summary>
    /// <param name="success">是否成功</param>
    public void RecordWrite(bool success)
    {
        TotalWriteCount++;
        if (!success)
        {
            WriteFailureCount++;
        }
    }

    /// <summary>
    /// 获取统计信息
    /// </summary>
    public DataModelStatistics GetStatistics()
    {
        return new DataModelStatistics
        {
            ModelId = DataModel.Id,
            ModelName = DataModel.Name,
            TotalVariableCount = TotalVariableCount,
            GoodVariableCount = GoodVariableCount,
            BadVariableCount = BadVariableCount,
            TotalDeviceCount = DeviceIds.Count,
            NormalDeviceCount = NormalDeviceCount,
            ErrorDeviceCount = ErrorDeviceCount,
            TotalReadCount = TotalReadCount,
            TotalWriteCount = TotalWriteCount,
            ReadFailureCount = ReadFailureCount,
            WriteFailureCount = WriteFailureCount,
            Status = Status,
            LastUpdateTime = LastUpdateTime
        };
    }
}

/// <summary>
/// 模型变量运行时类
/// </summary>
public class ModelVariableRuntime
{
    private readonly ModelVariable _variable;

    /// <summary>
    /// 模型变量实体
    /// </summary>
    public ModelVariable Variable { get; init; }

    /// <summary>
    /// 当前值
    /// </summary>
    public object? Value { get; set; }

    /// <summary>
    /// 上一个值
    /// </summary>
    public object? PreviousValue { get; set; }

    /// <summary>
    /// 质量状态
    /// </summary>
    public VariableQuality Quality { get; set; } = VariableQuality.Initializing;

    /// <summary>
    /// 是否变化
    /// </summary>
    public bool IsChanged { get; set; }

    /// <summary>
    /// 最后更新时间
    /// </summary>
    public DateTime? UpdateTime { get; set; }

    /// <summary>
    /// 最后读取时间
    /// </summary>
    public DateTime? LastReadTime { get; set; }

    /// <summary>
    /// 最后写入时间
    /// </summary>
    public DateTime? LastWriteTime { get; set; }

    /// <summary>
    /// 读取次数
    /// </summary>
    public long ReadCount { get; set; }

    /// <summary>
    /// 写入次数
    /// </summary>
    public long WriteCount { get; set; }

    /// <summary>
    /// 初始化模型变量运行时
    /// </summary>
    /// <param name="variable">模型变量实体</param>
    public ModelVariableRuntime(ModelVariable variable)
    {
        _variable = variable;
        Variable = variable;
    }

    /// <summary>
    /// 更新值
    /// </summary>
    /// <param name="newValue">新值</param>
    /// <param name="quality">质量状态</param>
    public void UpdateValue(object? newValue, VariableQuality quality)
    {
        PreviousValue = Value;
        Value = newValue;
        Quality = quality;
        IsChanged = !Equals(Value, PreviousValue);
        UpdateTime = DateTime.Now;
        LastReadTime = DateTime.Now;
        ReadCount++;
    }

    /// <summary>
    /// 记录写入
    /// </summary>
    /// <param name="newValue">写入的值</param>
    public void RecordWrite(object? newValue)
    {
        Value = newValue;
        LastWriteTime = DateTime.Now;
        WriteCount++;
    }

    /// <summary>
    /// 检查值是否在有效范围内
    /// </summary>
    public bool IsValueInRange(object? value)
    {
        if (value == null || Variable.Min == null || Variable.Max == null)
        {
            return true;
        }

        if (value is double dValue)
        {
            return dValue >= Variable.Min.Value && dValue <= Variable.Max.Value;
        }

        if (value is int iValue)
        {
            return iValue >= Variable.Min.Value && iValue <= Variable.Max.Value;
        }

        return true;
    }
}

/// <summary>
/// 数据模型状态枚举
/// </summary>
public enum DataModelStatus
{
    /// <summary>
    /// 正常
    /// </summary>
    Normal = 0,

    /// <summary>
    /// 警告（部分异常）
    /// </summary>
    Warning = 1,

    /// <summary>
    /// 错误（完全异常）
    /// </summary>
    Error = 2,

    /// <summary>
    /// 无设备关联
    /// </summary>
    NoDevices = 3
}

/// <summary>
/// 数据模型统计信息
/// </summary>
public class DataModelStatistics
{
    /// <summary>
    /// 模型ID
    /// </summary>
    public int ModelId { get; set; }

    /// <summary>
    /// 模型名称
    /// </summary>
    public string ModelName { get; set; } = string.Empty;

    /// <summary>
    /// 变量总数
    /// </summary>
    public int TotalVariableCount { get; set; }

    /// <summary>
    /// 正常变量数
    /// </summary>
    public int GoodVariableCount { get; set; }

    /// <summary>
    /// 异常变量数
    /// </summary>
    public int BadVariableCount { get; set; }

    /// <summary>
    /// 设备总数
    /// </summary>
    public int TotalDeviceCount { get; set; }

    /// <summary>
    /// 正常设备数
    /// </summary>
    public int NormalDeviceCount { get; set; }

    /// <summary>
    /// 异常设备数
    /// </summary>
    public int ErrorDeviceCount { get; set; }

    /// <summary>
    /// 总读取次数
    /// </summary>
    public long TotalReadCount { get; set; }

    /// <summary>
    /// 总写入次数
    /// </summary>
    public long TotalWriteCount { get; set; }

    /// <summary>
    /// 读取失败次数
    /// </summary>
    public long ReadFailureCount { get; set; }

    /// <summary>
    /// 写入失败次数
    /// </summary>
    public long WriteFailureCount { get; set; }

    /// <summary>
    /// 模型状态
    /// </summary>
    public DataModelStatus Status { get; set; }

    /// <summary>
    /// 最后更新时间
    /// </summary>
    public DateTime LastUpdateTime { get; set; }

    /// <summary>
    /// 读取成功率（百分比）
    /// </summary>
    public double ReadSuccessRate => TotalReadCount > 0 
        ? (double)(TotalReadCount - ReadFailureCount) / TotalReadCount * 100 
        : 100;

    /// <summary>
    /// 写入成功率（百分比）
    /// </summary>
    public double WriteSuccessRate => TotalWriteCount > 0 
        ? (double)(TotalWriteCount - WriteFailureCount) / TotalWriteCount * 100 
        : 100;
}
