public sealed class BmsWorker : BackgroundService
{
    private readonly IBmsHandlerRegistry _registry;
    private readonly ILogger<BmsWorker> _logger;

    public BmsWorker(
        IBmsHandlerRegistry registry,
        ILogger<BmsWorker> logger)
    {
        _registry = registry;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("BMS Worker starting...");

        var handlers = _registry.GetHandlers();

        foreach (var handler in handlers)
        {
            _logger.LogInformation("Starting handler for {IP}", handler.DeviceIP);
            await handler.StartAsync(stoppingToken);
        }

        // Just keep the service alive
        await Task.Delay(Timeout.Infinite, stoppingToken);
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("BMS Worker stopping...");

        var handlers = _registry.GetHandlers();

        foreach (var handler in handlers)
        {
            await handler.StopAsync();
        }

        await base.StopAsync(cancellationToken);
    }
}
