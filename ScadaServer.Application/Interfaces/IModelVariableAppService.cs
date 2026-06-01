using ScadaServer.Domain.Entities;
using ScadaServer.Application.DTOs;
namespace ScadaServer.Application.Interfaces
{
    public interface IModelVariableAppService
    {
        Task<ModelVariableDto> GetByIdAsync(int id);
        Task<List<ModelVariableDto>> GetListAsync();
        Task CreateAsync(ModelVariableDto dto);
        Task UpdateAsync(ModelVariableDto dto);
        Task DeleteAsync(int id);
    }
}

