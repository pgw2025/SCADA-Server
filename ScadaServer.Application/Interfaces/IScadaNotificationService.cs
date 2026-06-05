namespace ScadaServer.Application.Interfaces
{
    public interface IScadaNotificationService
    {
        Task NotifyVariableUpdateAsync(int deviceId, string variableKey, object value);
    }
}
