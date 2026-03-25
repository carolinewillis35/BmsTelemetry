public interface IBmsHandlerRegistry
{
    public void RegisterDevice(IBmsHandler handler);
    public IBmsHandler? GetBmsHandler(string deviceIP);
    public IReadOnlyCollection<IBmsHandler> GetHandlers();
}
