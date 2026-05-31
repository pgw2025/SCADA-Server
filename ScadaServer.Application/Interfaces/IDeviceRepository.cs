using ScadaServer.Domain.Entities;

namespace ScadaServer.Application.Interfaces
{
    public interface IDeviceRepository : IRepository<DeviceEntity>
    {
        Task<List<DeviceEntity>> GetActiveDevicesWithPointsAsync();
    }
}
