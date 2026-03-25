public class MockBmsHandler : IBmsHandler
{
    public string DeviceIP { get; init; }
    public ConnectionStatus Connection { get; init; }

    public MockBmsHandler(string deviceIP)
    {
        DeviceIP = deviceIP;
        Connection = ConnectionStatus.Unknown;
    }
}
