using System.Text.Json.Nodes;
using System.Xml.Linq;
using System.Xml;

public class DanfossProtocol
{
    private readonly IBmsTransport _transport;
    private readonly ILogger _logger;

    private readonly Dictionary<string, string> _requiredParams = new()
    {
        ["lang"] = "e",
        ["units"] = "U"
    };

    public DanfossProtocol(IBmsTransport transport, ILogger<DanfossProtocol> logger)
    {
        _transport = transport;
        _logger = logger;
    }

    public async Task<JsonNode?> SendCommandAsync(string action, IDictionary<string, string>? extraParams, CancellationToken ct)
    {
        var request = BuildRequest(action, extraParams);

        _logger.LogDebug($"Sending Danfoss command {action}");

        var response = await _transport.SendAsync(request, ct);

        if (response is null || !response.IsSuccessStatusCode)
        {
            _logger.LogWarning($"Danfoss command {action} failed. [{response?.StatusCode}]");
            return null;
        }

        return await TranslateAsync(response);
    }

    private HttpRequestMessage BuildRequest(string action, IDictionary<string, string>? extraParams)
    {
        var attributes = new List<XAttribute>();

        attributes.AddRange(_requiredParams.Select(kv => new XAttribute(kv.Key, kv.Value)));

        attributes.Add(new XAttribute("action", action));

        if (extraParams is not null)
        {
            attributes.AddRange(extraParams.Select(kv => new XAttribute(kv.Key, kv.Value)));
        }

        var element = new XElement("cmd", attributes);
        var xml = element.ToString(SaveOptions.DisableFormatting);

        var request = new HttpRequestMessage(HttpMethod.Post, "")
        {
            Content = new StringContent(xml, System.Text.Encoding.UTF8, "application/xml")
        };

        request.Headers.ConnectionClose = true;

        return request;
    }

    private async Task<JsonNode?> TranslateAsync(HttpResponseMessage response)
    {
        var xmlString = await response.Content.ReadAsStringAsync();

        var xmlDoc = new XmlDocument();
        xmlDoc.LoadXml(xmlString);

        var jsonText = Newtonsoft.Json.JsonConvert.SerializeXmlNode(xmlDoc);

        return JsonNode.Parse(jsonText);
    }
}
