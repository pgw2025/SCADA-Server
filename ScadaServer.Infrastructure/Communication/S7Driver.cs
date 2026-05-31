using ScadaServer.Domain.Entities;
using S7.Net;

namespace ScadaServer.Infrastructure.Communication
{
    public class S7Driver : IProtocolDriver
    {
        private Plc _plc;

        public async Task<bool> ConnectAsync(string connectionStr)
        {
            // Example: "192.168.1.20;Rack=0;Slot=1"
            var parts = connectionStr.Split(';');
            var ip = parts[0];
            short rack = 0;
            short slot = 1;
            
            foreach(var part in parts)
            {
                if(part.StartsWith("Rack=")) rack = short.Parse(part.Substring(5));
                if(part.StartsWith("Slot=")) slot = short.Parse(part.Substring(5));
            }

            _plc = new Plc(CpuType.S71200, ip, rack, slot);
            await _plc.OpenAsync();
            return _plc.IsConnected;
        }

        public async Task<object> ReadAsync(MetricConfig config)
        {
            if (_plc == null || !_plc.IsConnected) return null;
            return await _plc.ReadAsync(config.Address);
        }

        public async Task DisconnectAsync()
        {
            if (_plc != null) await Task.Run(() => _plc.Close());
        }
    }
}
