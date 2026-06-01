$files = @(
    "ExposedInterfaceAppService.cs", "HistoricalRecordAppService.cs", "HmiComponentAppService.cs",
    "ModelVariableAppService.cs", "MqttServerAppService.cs", "RealtimeDataAppService.cs",
    "ScadaPageAppService.cs", "ScadaProjectAppService.cs", "ScheduledTaskAppService.cs",
    "SensorAppService.cs", "SystemConfigAppService.cs", "SystemLogAppService.cs",
    "SystemScriptAppService.cs", "SystemUserAppService.cs", "VariableTriggerAppService.cs"
)

foreach ($file in $files) {
    $path = "ScadaServer.Application/Services/$file"
    if (Test-Path $path) {
        $content = Get-Content $path
        # Remove using
        $content = $content -replace "using ScadaServer.Infrastructure.Repositories;", ""
        # Replace Repository with IRepository (simple regex, needs verification)
        $content = $content -replace "([A-Z][a-zA-Z]+)Repository", "I$1Repository"
        $content | Set-Content $path
    }
}
