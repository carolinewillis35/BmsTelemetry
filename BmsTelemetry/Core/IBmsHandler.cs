public interface IBmsHandler
{
    string DeviceIP { get; init; }
    BmsType DeviceType { get; init; }
    ConnectionStatus Connection { get; init; }
    BmsHandlerStatus Status { get; init; }

    void Start();

    void Stop();
}
