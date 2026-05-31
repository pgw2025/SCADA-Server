namespace ScadaServer.Infrastructure.Communication
{
    public interface IDeviceRuntimeManager
    {
        void RefreshDevice(int deviceId);
        void ReloadAll();
    }
}
