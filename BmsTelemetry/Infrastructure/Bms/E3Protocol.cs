using System.Text.Json;
using System.Text.Json.Nodes;

public class E3Protocol
{
    private readonly IBmsTransport _transport;
    private readonly ILogger _logger;

    public E3Protocol(IBmsTransport transport, ILoggerFactory loggerFactory)
    {
        _transport = transport;
        _logger = loggerFactory.CreateLogger<E3Protocol>();
    }

    // MAIN ENTRY POINT
    public async Task<JsonNode?> SendCommandAsync(
        string method,
        JsonObject? parameters,
        CancellationToken ct,
        HttpMethod? httpMethod = null)
    {
        var request = BuildRequest(method, parameters, httpMethod ?? HttpMethod.Post);

        var response = await _transport.SendAsync(request, ct, method);

        if (response is null || !response.IsSuccessStatusCode)
        {
            return null;
        }

        return await TranslateAsync(response);
    }

    // REQUEST BUILDER (E3-specific weirdness)
    private HttpRequestMessage BuildRequest(
        string method,
        JsonObject? parameters,
        HttpMethod httpMethod)
    {
        object payload;

        if (parameters is not null)
        {
            var dict = parameters.ToDictionary(
                kvp => kvp.Key,
                kvp => kvp.Value is JsonValue v
                    ? v.GetValue<object>()
                    : kvp.Value
            );

            payload = new
            {
                jsonrpc = "2.0",
                method,
                id = "0",
                @params = dict
            };
        }
        else
        {
            payload = new
            {
                jsonrpc = "2.0",
                method,
                id = "0"
            };
        }

        var json = JsonSerializer.Serialize(payload);
        var encoded = Uri.EscapeDataString(json);
        var url = $"{_transport._endpoint}?m={encoded}";

        _logger.LogCritical(json.ToString());
        _logger.LogCritical(url.ToString());

        var request = new HttpRequestMessage(httpMethod, url);

        // Only POST gets a body
        if (httpMethod == HttpMethod.Post)
        {
            request.Content = new StringContent("");
        }

        request.Headers.ConnectionClose = true;

        return request;
    }

    // TRANSLATION
    private async Task<JsonNode?> TranslateAsync(HttpResponseMessage response)
    {
        var json = await response.Content.ReadAsStringAsync();

        try
        {
            return JsonNode.Parse(json);
        }
        catch
        {
            _logger.LogError("Failed to parse E3 response: {json}", json);
            return null;
        }
    }
}
