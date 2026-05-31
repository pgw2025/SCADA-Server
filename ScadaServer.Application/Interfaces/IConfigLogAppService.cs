namespace ScadaServer.Application.Interfaces
{
    public interface IConfigLogAppService
    {
        Task<ConfigLogDto> GetByIdAsync(int id);
        Task<List<ConfigLogDto>> GetListAsync();
        Task CreateAsync(ConfigLogDto dto);
        Task UpdateAsync(ConfigLogDto dto);
        Task DeleteAsync(int id);
    }
}
