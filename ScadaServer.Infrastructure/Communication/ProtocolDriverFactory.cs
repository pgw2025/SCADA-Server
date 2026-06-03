using ScadaServer.Domain.Entities;

namespace ScadaServer.Infrastructure.Communication
{
    public interface IProtocolDriverFactory
    {
        IProtocolDriver CreateDriver(string deviceType);
    }

    public class ProtocolDriverFactory : IProtocolDriverFactory
    {
        public IProtocolDriver CreateDriver(string deviceType)
        {
            return deviceType.ToUpper() switch
            {
                "S7" => new S7Driver(),
                "OPCUA" => new OpcUaDriver(),
                _ => null
            };
        }
    }
}
