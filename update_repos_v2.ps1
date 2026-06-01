$repos = @(
    "AlarmRule", "Area", "ConfigLog", "DatabaseConfig", "DataConversion", "DataModel",
    "ExposedInterface", "HistoricalRecord", "HmiComponent", "ModelVariable", "MqttServer",
    "RealtimeData", "ScadaPage", "ScadaProject", "ScheduledTask", "Sensor",
    "SystemConfig", "SystemLog", "SystemScript", "SystemUserRepository", "VariableTrigger"
)

# Actually, let's just list the files in the directory and process them.
$repoFiles = Get-ChildItem "ScadaServer.Infrastructure/Repositories/*.cs"

foreach ($file in $repoFiles) {
    $content = Get-Content $file.FullName
    $name = [System.IO.Path]::GetFileNameWithoutExtension($file.Name)
    
    # Ensure using statements
    if ($content -notmatch "using ScadaServer.Application.Interfaces;") {
        $content = "using ScadaServer.Application.Interfaces;`r`n" + $content
    }

    # Implement interface: class XRepository : SqlSugarRepository<Y> -> class XRepository : SqlSugarRepository<Y>, IXRepository
    $interfaceName = "I" + $name
    $content = $content -replace "class ($name)\s*:\s*SqlSugarRepository<([a-zA-Z0-9]+)>", "class `$1 : SqlSugarRepository<`$2>, $interfaceName"
    
    $content | Set-Content $file.FullName
}
