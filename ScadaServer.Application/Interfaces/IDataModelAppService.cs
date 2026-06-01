using ScadaServer.Domain.Entities;
using ScadaServer.Application.DTOs;
namespace ScadaServer.Application.Interfaces
{
    public interface IDataModelAppService
    {
        Task<DataModelDto> GetByIdAsync(int id);
        Task<List<DataModelDto>> GetListAsync();
        Task CreateAsync(DataModelDto dto);
        Task UpdateAsync(DataModelDto dto);
        Task DeleteAsync(int id);
    }
}

