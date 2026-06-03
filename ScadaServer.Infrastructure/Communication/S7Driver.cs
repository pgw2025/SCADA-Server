using System.Text.RegularExpressions;
using S7.Net;
using ScadaServer.Domain.Entities;

namespace ScadaServer.Infrastructure.Communication
{
    public class S7Driver : IProtocolDriver
    {
        private Plc _plc;
        private static readonly Regex S7AddressRegex = new Regex(@"DB(?<db>\d+)\.(?<type>[A-Z]+)(?<offset>\d+)(\.(?<bit>\d+))?", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public async Task<bool> ConnectAsync(Device device)
        {
            var cpuType = device.CpuType switch
            {
                "S71200" => CpuType.S71200,
                "S71500" => CpuType.S71500,
                "S7300" => CpuType.S7300,
                "S7400" => CpuType.S7400,
                _ => CpuType.S71200
            };

            _plc = new Plc(cpuType, device.IpAddress, (short)(device.Rack ?? 0), (short)(device.Slot ?? 1));
            await _plc.OpenAsync();
            return _plc.IsConnected;
        }

        public async Task<object> ReadAsync(ModelVariable variable)
        {
            if (_plc == null || !_plc.IsConnected) return null;
            return await _plc.ReadAsync(variable.Address);
        }

        public async Task<IDictionary<string, object>> ReadBatchAsync(IEnumerable<ModelVariable> variables)
        {
            var results = new Dictionary<string, object>();
            if (_plc == null || !_plc.IsConnected) return results;

            // Group by DB block
            var groups = variables.Select(v => new { Variable = v, Info = ParseAddress(v.Address) })
                                 .Where(x => x.Info != null)
                                 .GroupBy(x => x.Info.DbNumber);

            foreach (var group in groups)
            {
                int dbNumber = group.Key;
                var varInfos = group.ToList();

                // Calculate range
                int minOffset = varInfos.Min(x => x.Info.ByteOffset);
                int maxOffset = varInfos.Max(x => x.Info.ByteOffset + x.Info.ByteLength);
                int length = maxOffset - minOffset;

                // S7.Net Max PDU size is usually 240-480 bytes, but ReadBytes handles fragmentation if needed
                // However, for efficiency, we might want to split if the gap is too large. 
                // For now, we'll read the whole range.
                byte[] buffer = await _plc.ReadBytesAsync(DataType.DataBlock, dbNumber, minOffset, length);

                foreach (var item in varInfos)
                {
                    var info = item.Info;
                    var variable = item.Variable;
                    int relativeOffset = info.ByteOffset - minOffset;

                    object value = info.DataType switch
                    {
                        "DBX" => (buffer[relativeOffset] & (1 << info.BitOffset)) != 0,
                        "DBB" => buffer[relativeOffset],
                        "DBW" => S7.Net.Types.Int.FromByteArray(new byte[] { buffer[relativeOffset], buffer[relativeOffset + 1] }),
                        "DBD" => info.IsReal ? S7.Net.Types.Real.FromByteArray(new byte[] { buffer[relativeOffset], buffer[relativeOffset + 1], buffer[relativeOffset + 2], buffer[relativeOffset + 3] }) 
                                            : S7.Net.Types.DInt.FromByteArray(new byte[] { buffer[relativeOffset], buffer[relativeOffset + 1], buffer[relativeOffset + 2], buffer[relativeOffset + 3] }),
                        _ => null
                    };

                    if (value != null)
                    {
                        results[variable.Key] = value;
                    }
                }
            }

            return results;
        }

        public Task SubscribeAsync(IEnumerable<ModelVariable> variables, Action<string, object> onValueChanged)
        {
            // S7 doesn't support native subscription. Worker will handle polling.
            return Task.CompletedTask;
        }

        public Task UnsubscribeAsync(IEnumerable<ModelVariable> variables)
        {
            return Task.CompletedTask;
        }

        public async Task DisconnectAsync()
        {
            if (_plc != null)
            {
                await Task.Run(() => _plc.Close());
                _plc = null;
            }
        }

        private S7AddressInfo ParseAddress(string address)
        {
            var match = S7AddressRegex.Match(address);
            if (!match.Success) return null;

            int db = int.Parse(match.Groups["db"].Value);
            string type = match.Groups["type"].Value.ToUpper();
            int offset = int.Parse(match.Groups["offset"].Value);
            int bit = match.Groups["bit"].Success ? int.Parse(match.Groups["bit"].Value) : 0;

            int byteLength = type switch
            {
                "DBX" => 1,
                "DBB" => 1,
                "DBW" => 2,
                "DBD" => 4,
                _ => 1
            };

            return new S7AddressInfo
            {
                DbNumber = db,
                DataType = type,
                ByteOffset = offset,
                BitOffset = bit,
                ByteLength = byteLength,
                IsReal = type == "DBD" // Default to DInt or Real based on some heuristic or variable Type
            };
        }

        private class S7AddressInfo
        {
            public int DbNumber { get; set; }
            public string DataType { get; set; }
            public int ByteOffset { get; set; }
            public int BitOffset { get; set; }
            public int ByteLength { get; set; }
            public bool IsReal { get; set; }
        }
    }
}
