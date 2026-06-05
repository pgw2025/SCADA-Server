using Microsoft.AspNetCore.SignalR;
using ScadaServer.Application.Interfaces;
using ScadaServer.WebApi.Hubs;

namespace ScadaServer.WebApi.Services
{
    public class SignalRNotificationService : IScadaNotificationService
    {
        private readonly IHubContext<ScadaHub> _hubContext;
        private readonly IMqttManager _mqttManager;

        public SignalRNotificationService(IHubContext<ScadaHub> hubContext, IMqttManager mqttManager)
        {
            _hubContext = hubContext;
            _mqttManager = mqttManager;
        }

        public async Task NotifyVariableUpdateAsync(int deviceId, string variableKey, object value)
        {
            // SignalR notification
            await _hubContext.Clients.All.SendAsync("ReceiveVariableUpdate", deviceId, variableKey, value);
            
            // MQTT notification
            await _mqttManager.PublishVariableUpdateAsync(deviceId, variableKey, value);
        }
    }
}
