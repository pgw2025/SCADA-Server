using ScadaServer.Application.DTOs;
using ScadaServer.Domain.Entities;

namespace ScadaServer.Application.Interfaces
{
    public interface IDeviceRepository : IRepository<Device>
    {
        /// <summary>
        /// 获取所有启用的设备
        /// </summary>
        Task<List<Device>> GetActiveDevicesAsync();

        /// <summary>
        /// 根据ID获取设备（包含协议配置）
        /// </summary>
        Task<Device?> GetByIdWithConfigAsync(int id);

        /// <summary>
        /// 获取所有设备列表（包含协议配置）
        /// </summary>
        Task<List<Device>> GetListWithConfigAsync();

        /// <summary>
        /// 根据Key获取设备
        /// </summary>
        Task<Device?> GetByKeyAsync(string key);
    }
}
