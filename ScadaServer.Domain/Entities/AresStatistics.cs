namespace ScadaServer.Domain.Entities
{
    public class AreaStatistics
    {
        public int DeviceCount { get; set; }

        public int OnlineDeviceCount { get; set; }

        public int OfflineDeviceCount { get; set; }

        public int VariableCount { get; set; }

        public int AlarmCount { get; set; }

        public DateTime UpdateTime { get; set; }
    }
}