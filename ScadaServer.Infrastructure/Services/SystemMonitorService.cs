using Microsoft.Extensions.Hosting;
using ScadaServer.Application.DTOs;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace ScadaServer.Infrastructure.Services;

public class SystemMonitorService : BackgroundService
{
    // 将计数器设为静态，以便其他类可以访问或更新
    private static long _totalPollPackets = 0;
    public static long TotalPollPackets => _totalPollPackets;

    public SystemStatusDto CurrentStatus { get; private set; } = new();

    private readonly PerformanceCounter? _cpuCounter;
    private readonly PerformanceCounter? _ramCounter;
    private readonly PerformanceCounter? _diskTimeCounter;
    
    private readonly List<PerformanceCounter> _netInCounters = new();
    private readonly List<PerformanceCounter> _netOutCounters = new();

    public static void IncrementPollPackets()
    {
        Interlocked.Increment(ref _totalPollPackets);
    }

    public SystemMonitorService()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            try
            {
                _cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
                _ramCounter = new PerformanceCounter("Memory", "Available MBytes");
                _diskTimeCounter = new PerformanceCounter("PhysicalDisk", "% Disk Time", "_Total");

                var category = new PerformanceCounterCategory("Network Interface");
                foreach (var instance in category.GetInstanceNames())
                {
                    _netInCounters.Add(new PerformanceCounter("Network Interface", "Bytes Received/sec", instance));
                    _netOutCounters.Add(new PerformanceCounter("Network Interface", "Bytes Sent/sec", instance));
                }
            }
            catch (Exception)
            {
                // In case of insufficient permissions or system limitations
            }
        }
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var process = Process.GetCurrentProcess();
        
        while (!stoppingToken.IsCancellationRequested)
        {
            var uptime = DateTime.Now - process.StartTime;

            double cpu = 0.0;
            double memPercent = 0.0;
            double diskTime = 0.0;
            double netIn = 0.0;
            double netOut = 0.0;

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                cpu = _cpuCounter?.NextValue() ?? 0.0;
                
                var memInfo = GC.GetGCMemoryInfo();
                long totalBytes = memInfo.TotalAvailableMemoryBytes;
                float availableMb = _ramCounter?.NextValue() ?? 0.0f;
                double totalMb = totalBytes / (1024.0 * 1024.0);
                
                if (totalMb > 0)
                {
                    memPercent = ((totalMb - availableMb) / totalMb) * 100;
                }

                diskTime = _diskTimeCounter?.NextValue() ?? 0.0;
                netIn = _netInCounters.Sum(c => c.NextValue()) / 1024.0; // KB/s
                netOut = _netOutCounters.Sum(c => c.NextValue()) / 1024.0; // KB/s
            }

            CurrentStatus = new SystemStatusDto
            {
                CpuUsage = Math.Round(cpu, 2),
                MemUsage = Math.Round(memPercent, 2),
                DiskLoadPercentage = Math.Round(diskTime, 2),
                NetworkIn = Math.Round(netIn, 2),
                NetworkOut = Math.Round(netOut, 2),
                UptimeDays = uptime.Days,
                UptimeHours = uptime.Hours,
                UptimeMins = uptime.Minutes,
                PollFreq = 2000,
                TotalPollPackets = TotalPollPackets, 
                Disks = GetDiskMetrics()
            };

            await Task.Delay(2000, stoppingToken);
        }
    }

    private List<DiskInfoDto> GetDiskMetrics()
    {
        var diskList = new List<DiskInfoDto>();
        var drives = DriveInfo.GetDrives().Where(d => d.DriveType == DriveType.Fixed && d.IsReady);

        foreach (var drive in drives)
        {
            long totalSize = drive.TotalSize;
            long freeSpace = drive.AvailableFreeSpace;
            long usedSize = totalSize - freeSpace;

            diskList.Add(new DiskInfoDto
            {
                Name = drive.Name,
                Label = drive.VolumeLabel,
                TotalSizeGb = totalSize / 1024 / 1024 / 1024,
                UsedSizeGb = usedSize / 1024 / 1024 / 1024,
                UsagePercentage = Math.Round((double)usedSize / totalSize * 100, 2)
            });
        }
        return diskList;
    }

    public override void Dispose()
    {
        _cpuCounter?.Dispose();
        _ramCounter?.Dispose();
        _diskTimeCounter?.Dispose();
        foreach (var c in _netInCounters) c.Dispose();
        foreach (var c in _netOutCounters) c.Dispose();
        base.Dispose();
    }
}
