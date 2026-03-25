public static class IBmsHandlerFactory
{
    public static IBmsHandler Create(DeviceSettings deviceSettings)
    {
        var client = IBmsHandlerFactory.GetClientForSettings(deviceSettings);
        return new BmsHandler(deviceSettings, client);
    }

    private static IBmsClient GetClientForSettings(DeviceSettings deviceSettings)
    {
        switch (deviceSettings.device_type)
        {
            case BmsType.EmersonE2:
                return new E2DeviceClient();
            default:
                throw new NotImplementedException($"Device type {deviceSettings.device_type} is not implemented!");
        }
    }
}
