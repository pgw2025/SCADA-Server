$files = Get-ChildItem -Path "ScadaServer.Infrastructure/**/*.cs" -Recurse

foreach ($file in $files) {
    $content = Get-Content $file.FullName -Raw
    
    $changed = $false
    
    # 1. Replace DeviceEntity with Device
    if ($content -match "DeviceEntity") {
        $content = $content -replace "DeviceEntity", "Device"
        $changed = $true
    }

    # 2. Fix DeviceWorker dependencies
    if ($file.Name -eq "DeviceWorker.cs") {
        $content = $content -replace "using ScadaServer\.WebApi\.Hubs;", ""
        $content = $content -replace "using Microsoft\.AspNetCore\.SignalR;", ""
        $content = $content -replace "private readonly IHubContext<ScadaHub> _hubContext;", "private readonly IScadaNotificationService _notificationService;"
        $content = $content -replace "IHubContext<ScadaHub> hubContext", "IScadaNotificationService notificationService"
        $content = $content -replace "_hubContext = hubContext;", "_notificationService = notificationService;"
        $content = $content -replace "_hubContext\.Clients\.All\.SendAsync\(`"ReceiveVariableUpdate`", v\.Key, val\)", "_notificationService.NotifyVariableUpdateAsync(v.Key, val)"
        $content = $content -replace "_hubContext\.Clients\.All\.SendAsync\(`"ReceiveVariableUpdate`", v\.Key, val\)", "_notificationService.NotifyVariableUpdateAsync(v.Key, val)"
        $changed = $true
    }
    
    if ($changed) {
        $content | Set-Content $file.FullName
    }
}
