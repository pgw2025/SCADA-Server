namespace ScadaServer.Domain.Exceptions
{
    public class DeviceNotFoundException : Exception
    {
        public DeviceNotFoundException(int deviceId) 
            : base($"Device with ID {deviceId} was not found.") { }
    }
}
