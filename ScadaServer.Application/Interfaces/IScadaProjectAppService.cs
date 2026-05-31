namespace ScadaServer.Application.Interfaces
{
    public interface IScadaProjectAppService
    {
        Task<ScadaProjectDto> GetByIdAsync(int id);
        Task<List<ScadaProjectDto>> GetListAsync();
        Task CreateAsync(ScadaProjectDto dto);
        Task UpdateAsync(ScadaProjectDto dto);
        Task DeleteAsync(int id);
    }
}
