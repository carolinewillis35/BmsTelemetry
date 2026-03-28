public class TelemetryRecord
{
    public int Id { get; set; }  // Primary key

    public string Ip { get; set; } = string.Empty;
    public string DeviceKey { get; set; } = string.Empty;

    public string DataKey { get; set; } = string.Empty;
    public string DataValue { get; set; } = string.Empty;

    public DateTime Timestamp { get; set; }  // When inserted
}
