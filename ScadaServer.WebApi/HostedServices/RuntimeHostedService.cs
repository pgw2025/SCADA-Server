using Microsoft.Extensions.Hosting;
using ScadaServer.Runtime;

namespace ScadaServer.WebApi.HostedServices;

public class RuntimeHostedService : BackgroundService
{
    private readonly RuntimeManager _runtimeManager;
    private readonly ILogger<RuntimeHostedService> _logger;

    public RuntimeHostedService(
        RuntimeManager runtimeManager,
        ILogger<RuntimeHostedService> logger)
    {
        _runtimeManager = runtimeManager;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(
        CancellationToken stoppingToken)
    {
        _logger.LogInformation("SCADA Runtime Starting...");

        try
        {
            await _runtimeManager.InitializeAsync();

            // await _runtimeManager.StartAsync(stoppingToken);

            _logger.LogInformation("SCADA Runtime Started");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Runtime Start Failed");
        }
    }

    public override async Task StopAsync(
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("SCADA Runtime Stopping...");

        // await _runtimeManager.StopAsync();

        await base.StopAsync(cancellationToken);
    }
}