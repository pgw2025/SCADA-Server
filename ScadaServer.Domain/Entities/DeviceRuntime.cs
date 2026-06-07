using ScadaServer.Domain.Enums;

namespace ScadaServer.Domain.Entities
{
    /// <summary>
    /// 设备运行时状态 - 仅内存维护，不持久化
    /// </summary>
    public class DeviceRuntime
    {
        /// <summary>
        /// 设备ID
        /// </summary>
        public int DeviceId { get; set; }

        /// <summary>
        /// 设备名称（缓存）
        /// </summary>
        public string DeviceName { get; set; } = string.Empty;

        /// <summary>
        /// 当前状态
        /// </summary>
        public DeviceStatus CurrentStatus { get; set; } = DeviceStatus.Offline;

        /// <summary>
        /// 最后一次心跳时间
        /// </summary>
        public DateTime? LastHeartbeat { get; set; }

        /// <summary>
        /// 重连次数
        /// </summary>
        public int ReconnectCount { get; set; }

        /// <summary>
        /// 最后一次错误信息
        /// </summary>
        public string? LastError { get; set; }

        /// <summary>
        /// 最后一次通信时间
        /// </summary>
        public DateTime? LastCommunicationTime { get; set; }

        /// <summary>
        /// 是否正在连接中
        /// </summary>
        public bool IsConnecting { get; set; }

        /// <summary>
        /// 运行时长（秒）
        /// </summary>
        public long UptimeSeconds { get; set; }

        /// <summary>
        /// 采集统计：成功次数
        /// </summary>
        public long SuccessCount { get; set; }

        /// <summary>
        /// 采集统计：失败次数
        /// </summary>
        public long FailureCount { get; set; }
    }
}
