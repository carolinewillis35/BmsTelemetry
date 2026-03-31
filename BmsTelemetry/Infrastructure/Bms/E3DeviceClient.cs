using System.Runtime.CompilerServices;
using System.Text.Json.Nodes;

public sealed class E3DeviceClient : IBmsClient
{
    private readonly E3Protocol _protocol;
    private readonly ILogger<E3DeviceClient> _logger;
    private readonly IBmsTransport _transport;
    private readonly DbReader _dbReader;
    private readonly string _ip;
    private DateTime _lastInitTime = DateTime.MinValue;

    public E3DeviceClient(IBmsTransport transport, string deviceIP, DbReader dbReader, ILoggerFactory loggerFactory)
    {
        _logger = loggerFactory.CreateLogger<E3DeviceClient>();
        _protocol = new E3Protocol(transport, loggerFactory);
        _transport = transport;
        _dbReader = dbReader;
        _ip = deviceIP;
    }

    public async IAsyncEnumerable<ClientCommand> GetPollingSequenceAsync([EnumeratorCancellation] CancellationToken ct)
    {
        // if (DateTime.UtcNow - _lastInitTime >= TimeSpan.FromMinutes(60))
        // {
        //     foreach (var cmd in GetInitCommands())
        //     {
        //         yield return cmd;
        //     }
        //
        //     _lastInitTime = DateTime.UtcNow;
        // }
        //
        // foreach (var cmd in GetContinuousCommands())
        // {
        //     yield return cmd;
        // }

        // Test new commands
        yield return new ClientCommand(
            "GetSessionIdAsync",
            ct => GetSessionIdAsync(ct)
        );
    }

    private IEnumerable<ClientCommand> GetInitCommands()
    {
        yield return new ClientCommand(
            "GetSessionIdAsync",
            ct => GetSessionIdAsync(ct)
        );
    }

    private IEnumerable<ClientCommand> GetContinuousCommands()
    {
        yield return new ClientCommand(
            "GetSessionIdAsync",
            ct => GetSessionIdAsync(ct)
        );
    }

    private async Task<JsonNode?> GetSessionIdAsync(CancellationToken ct)
    {
        var response = await _protocol.SendCommandAsync("GetSessionID", null, ct, HttpMethod.Get);
        if (response is null)
            return null;

        NormalizerService.ConsolePrettyPrint(response);
        return null;
    }
}
