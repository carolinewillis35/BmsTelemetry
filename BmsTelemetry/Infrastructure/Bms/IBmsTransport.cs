public interface IBmsTransport
{
    Uri _endpoint { get; init; }
    Task<HttpResponseMessage?> SendAsync(HttpRequestMessage request, CancellationToken ct = default, string? context = null);
}
