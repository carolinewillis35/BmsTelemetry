using System.Runtime.CompilerServices;
using System.Text.Json.Nodes;

public sealed class E2DeviceClient : IBmsClient
{
    private readonly E2Protocol _protocol;
    private readonly ILogger<E2DeviceClient> _logger;
    private readonly IBmsTransport _transport;
    private readonly DbReader _dbReader;
    private readonly string _ip;
    private string _controllerName = "";
    private DateTime _lastInitTime = DateTime.MinValue;

    public E2DeviceClient(IBmsTransport transport, string deviceIP, DbReader dbReader, ILoggerFactory loggerFactory)
    {
        _logger = loggerFactory.CreateLogger<E2DeviceClient>();
        _protocol = new E2Protocol(transport, loggerFactory);
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
            "E2.GetCellListAsync",
            ct => GetCellListAsync(ct)
        );
    }

    private IEnumerable<ClientCommand> GetInitCommands()
    {
        yield return new ClientCommand(
            "E2.GetControllerListAsync",
            ct => GetControllerListAsync(ct)
        );

        yield return new ClientCommand(
            "E2.GetCellListAsync",
            ct => GetCellListAsync(ct)
        );
    }

    private IEnumerable<ClientCommand> GetContinuousCommands()
    {
        yield return new ClientCommand(
            "E2.GetControllerListAsync",
            ct => GetControllerListAsync(ct)
        );
    }

    private async Task<JsonNode?> GetControllerListAsync(CancellationToken ct)
    {
        var response = await _protocol.SendCommandAsync("E2.GetControllerList", null, ct);
        if (response is null)
            return null;

        var controllerArr = response["result"]?.AsArray() ?? new JsonArray();

        var outArr = new JsonArray();

        int i = 0;
        foreach (var controller in controllerArr)
        {
            var name = controller?["name"]?.GetValue<string>();

            if (string.IsNullOrEmpty(name))
                continue;

            outArr.Add(new JsonObject
            {
                ["device_key"] = $"controller{name}",
                ["data"] = controller?.DeepClone() ?? new JsonObject()
            });

            // Set primary controller
            if (i == 0)
                _controllerName = name;
            i++;
        }

        var result = new JsonObject { ["data"] = outArr };

        return result;
    }

    private async Task<JsonNode?> GetCellListAsync(CancellationToken ct)
    {
        var response = await _protocol.SendCommandAsync("E2.GetCellList", new JsonArray { _controllerName }, ct);
        if (response is null)
            return null;

        var cellArr = response["result"]?["data"]?.AsArray() ?? new JsonArray();
        var outArr = new JsonArray();

        foreach (var cell in cellArr)
        {
            var name = cell?["cellname"]?.GetValue<string>();

            if (string.IsNullOrEmpty(name))
                continue;

            outArr.Add(new JsonObject
            {
                ["device_key"] = $"controller{_controllerName}:cell{name}",
                ["data"] = cell?.DeepClone() ?? new JsonObject()
            });
        }

        var result = new JsonObject { ["data"] = outArr };

        return result;
    }
}
