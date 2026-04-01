using System.Text.Json.Nodes;

public interface IIotDevice
{
    event Action? OnStatusChanged;

    ConnectionStatus Connected { get; }
    int TotalMessagesSent { get; }
    string Type { get; }

    Task ConnectAsync(CancellationToken ct = default);
    Task SendMessageAsync(JsonNode payload, CancellationToken ct = default);
    Task DisconnectAsync(CancellationToken ct = default);
}
