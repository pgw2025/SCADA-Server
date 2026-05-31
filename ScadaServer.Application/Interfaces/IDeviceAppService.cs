namespace ScadaServer.Application.Interfaces
{
    public interface IDeviceAppService
    {
        Task<DeviceDto> GetByIdAsync(int id);
        Task<List<DeviceDto>> GetListAsync();
        Task CreateAsync(DeviceDto dto);
        Task UpdateAsync(DeviceDto dto);
        Task DeleteAsync(int id);
    }
}
