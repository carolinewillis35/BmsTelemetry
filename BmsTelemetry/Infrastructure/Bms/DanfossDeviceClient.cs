using System.Text.Json.Nodes;
using System.Runtime.CompilerServices;

public sealed class DanfossDeviceClient : IBmsClient
{
    private readonly DanfossProtocol _protocol;
    private readonly ILogger<DanfossDeviceClient> _logger;
    private readonly IBmsTransport _transport;
    private readonly IIotDevice _iotDevice;

    public DanfossDeviceClient(IBmsTransport transport, ILoggerFactory loggerFactory, IIotDevice iotDevice)
    {
        _logger = loggerFactory.CreateLogger<DanfossDeviceClient>();
        _protocol = new DanfossProtocol(transport, loggerFactory);
        _iotDevice = iotDevice;
        _transport = transport;
    }

    public async IAsyncEnumerable<ClientCommand> GetPollingSequenceAsync([EnumeratorCancellation] CancellationToken ct)
    {
        yield return new ClientCommand(
            "ReadSensorsAsync",
            async ct2 => await ReadSensorsAsync(ct2)
        );
    }

    // Interaction methods

    private Task<JsonNode?> ReadSensorsAsync(CancellationToken ct)
    {
        return _protocol.SendCommandAsync("read_sensors", null, ct);
    }

    // private Task<JsonNode?> ReadHvacUnitAsync(string ahindex, CancellationToken ct)
    // {
    //     return _protocol.SendCommandAsync("read_hvac_unit", new Dictionary<string, string>() { ["ahindex"] = ahindex }, ct);
    // }

}
