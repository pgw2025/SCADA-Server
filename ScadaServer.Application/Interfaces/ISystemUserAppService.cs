namespace ScadaServer.Application.Interfaces
{
    public interface ISystemUserAppService
    {
        Task<SystemUserDto> GetByIdAsync(int id);
        Task<List<SystemUserDto>> GetListAsync();
        Task CreateAsync(SystemUserDto dto);
        Task UpdateAsync(SystemUserDto dto);
        Task DeleteAsync(int id);
    }
}
