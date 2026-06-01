$repositories = @(
    "AlarmRuleRepository",
    "AreaRepository",
    "ConfigLogRepository",
    "DatabaseConfigRepository",
    "DataConversionRepository",
    "DataModelRepository",
    "ExposedInterfaceRepository",
    "HistoricalRecordRepository",
    "HmiComponentRepository",
    "ModelVariableRepository",
    "MqttServerRepository",
    "RealtimeDataRepository",
    "ScadaPageRepository",
    "ScadaProjectRepository",
    "ScheduledTaskRepository",
    "SystemConfigRepository",
    "SystemLogRepository",
    "SystemScriptRepository",
    "SystemUserRepository",
    "VariableTriggerRepository"
)

foreach ($repoName in $repositories) {
    $filePath = "D:\Web\SCADA\Server\ScadaServer.Infrastructure\Repositories\$repoName.cs"
    if (Test-Path $filePath) {
        $content = Get-Content $filePath -Raw
        
        # Add using statement if not exists
        if ($content -notmatch "using ScadaServer.Application.Interfaces;") {
            $content = "using ScadaServer.Application.Interfaces;`r`n" + $content
        }
        
        # Replace class definition
        $interfaceName = "I" + $repoName
        $pattern = "class $repoName : SqlSugarRepository<(\w+)>"
        $replacement = "class $repoName : SqlSugarRepository<`$1>, $interfaceName"
        
        $content = $content -replace $pattern, $replacement
        
        Set-Content $filePath -Value $content
    }
}
