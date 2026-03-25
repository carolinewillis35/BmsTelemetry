public class BmsHandler : IBmsHandler
{
    public string DeviceIP { get; init; }
    public BmsType DeviceType { get; init; }
    public ConnectionStatus Connection { get; init; }
    public BmsHandlerStatus Status { get; init; }

    private readonly IBmsClient _bmsClient;

    public BmsHandler(DeviceSettings deviceSettings, IBmsClient bmsClient)
    {
        DeviceIP = deviceSettings.IP;
        DeviceType = deviceSettings.device_type;
        Connection = ConnectionStatus.Unknown;
        Status = BmsHandlerStatus.Idle;

        _bmsClient = bmsClient;
    }

    public void Start()
    {
        _bmsClient.Start();
    }

    public void Stop()
    {
        _bmsClient.Stop();
    }
}
