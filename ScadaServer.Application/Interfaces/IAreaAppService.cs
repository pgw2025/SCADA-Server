namespace ScadaServer.Application.Interfaces
{
    public interface IAreaAppService
    {
        Task<AreaDto> GetByIdAsync(int id);
        Task<List<AreaDto>> GetListAsync();
        Task CreateAsync(AreaDto dto);
        Task UpdateAsync(AreaDto dto);
        Task DeleteAsync(int id);
    }
}
