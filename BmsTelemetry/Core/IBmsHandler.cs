public interface IBmsHandler
{
    string DeviceIP { get; init; }
    ConnectionStatus Connection { get; init; }
}
