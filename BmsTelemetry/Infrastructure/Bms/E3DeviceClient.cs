using System.Runtime.CompilerServices;
using System.Text.Json.Nodes;

public sealed class E3DeviceClient : IBmsClient
{
    private readonly E3Protocol _protocol;
    private readonly ILogger<E3DeviceClient> _logger;
    private readonly IBmsTransport _transport;
    private readonly DbReader _dbReader;
    private readonly string _ip;
    private string _sessionId = string.Empty;
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
            "GetSystemInventoryAsync",
            ct => GetSystemInventoryAsync(ct)
        );
    }

    private IEnumerable<ClientCommand> GetInitCommands()
    {
        yield return new ClientCommand(
            "GetSystemInventoryAsync",
            ct => GetSystemInventoryAsync(ct)
        );
    }

    private IEnumerable<ClientCommand> GetContinuousCommands()
    {
        yield return new ClientCommand(
            "GetSystemInventoryAsync",
            ct => GetSystemInventoryAsync(ct)
        );
    }

    private async Task<JsonNode?> GetSystemInventoryAsync(CancellationToken ct)
    {
        var sidOk = await EnsureSessionIdAsync(ct);
        if (!sidOk)
            return null;

        var response = await _protocol.SendCommandAsync(
            "GetSystemInventory",
            new JsonObject { ["sid"] = _sessionId },
            ct,
            HttpMethod.Post
        );

        if (response is null)
        {
            _sessionId = string.Empty;
            return null;
        }

        // Validate JSON structure
        var result = response["result"] as JsonObject;
        if (result is null)
        {
            _sessionId = string.Empty;
            return null;
        }

        var aps = result["aps"] as JsonArray;
        if (aps is null)
        {
            _sessionId = string.Empty;
            return null;
        }

        // Build output
        var outArr = new JsonArray();

        foreach (var entry in aps)
        {
            if (entry is not JsonObject obj)
                continue;

            var appName = obj["appname"]?.GetValue<string>();
            if (string.IsNullOrEmpty(appName))
                continue;

            outArr.Add(new JsonObject
            {
                ["device_key"] = $"app{appName}",
                ["data"] = obj.DeepClone()
            });
        }

        var outObj = new JsonObject
        {
            ["data"] = outArr
        };

        return outObj;
    }

    private async Task<bool> EnsureSessionIdAsync(CancellationToken ct)
    {
        if (string.IsNullOrEmpty(_sessionId))
        {
            await GetSessionIdAsync(ct);

            if (string.IsNullOrEmpty(_sessionId))
                return false;
        }
        return true;
    }

    private async Task GetSessionIdAsync(CancellationToken ct)
    {
        _logger.LogInformation("Fetching new SID for {_endpoint}", _transport._endpoint);
        var response = await _protocol.SendCommandAsync("GetSessionID", null, ct, HttpMethod.Get);
        if (response is null)
        {
            _sessionId = string.Empty;
            return;
        }

        _sessionId = response["result"]?["sid"]?.GetValue<string>() ?? string.Empty;

        return;
    }
}
