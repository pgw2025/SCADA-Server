namespace ScadaServer.Application.Interfaces
{
    public interface IDeviceAppService
    {
        Task UpdateDeviceConfigTxAsync(int deviceId, string newAddress);
    }
}
