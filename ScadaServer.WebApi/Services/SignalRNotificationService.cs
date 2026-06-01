using Microsoft.AspNetCore.SignalR;
using ScadaServer.Application.Interfaces;
using ScadaServer.WebApi.Hubs;

namespace ScadaServer.WebApi.Services
{
    public class SignalRNotificationService : IScadaNotificationService
    {
        private readonly IHubContext<ScadaHub> _hubContext;

        public SignalRNotificationService(IHubContext<ScadaHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task NotifyVariableUpdateAsync(string variableKey, object value)
        {
            await _hubContext.Clients.All.SendAsync("ReceiveVariableUpdate", variableKey, value);
        }
    }
}
