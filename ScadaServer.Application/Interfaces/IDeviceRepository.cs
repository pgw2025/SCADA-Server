using ScadaServer.Application.DTOs;
using ScadaServer.Domain.Entities;

namespace ScadaServer.Application.Interfaces
{
    public interface IDeviceRepository : IRepository<Device>
    {
        Task<List<Device>> GetActiveDevicesAsync();
    }
}

