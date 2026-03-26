using Microsoft.Extensions.Options;

public class BmsHandlerRegistry : IBmsHandlerRegistry
{
    private Dictionary<string, IBmsHandler> bmsHandlers = new();
    private readonly IBmsHandlerFactory _bmsHandlerFactory;

    public BmsHandlerRegistry(IOptions<NetworkSettings> networkSettings, IOptions<GeneralSettings> generalSettings, IBmsHandlerFactory bmsHandlerFactory)
    {
        var netsettings = networkSettings.Value.bms_devices;
        var gensettings = generalSettings.Value;
        _bmsHandlerFactory = bmsHandlerFactory;

        foreach (var entry in netsettings)
        {
            var handler = _bmsHandlerFactory.Create(entry);
            RegisterDevice(handler);
        }
    }

    public void RegisterDevice(IBmsHandler handler)
    {
        bmsHandlers.TryAdd(handler.DeviceIP, handler);
    }

    public IBmsHandler? GetBmsHandler(string deviceIP)
    {
        return bmsHandlers.GetValueOrDefault(deviceIP);
    }

    public IReadOnlyCollection<IBmsHandler> GetHandlers()
    {
        return bmsHandlers.Values;
    }
}
