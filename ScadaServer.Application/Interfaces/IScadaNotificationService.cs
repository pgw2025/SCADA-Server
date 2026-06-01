namespace ScadaServer.Application.Interfaces
{
    public interface IScadaNotificationService
    {
        Task NotifyVariableUpdateAsync(string variableKey, object value);
    }
}
