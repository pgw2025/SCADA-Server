using ScadaServer.Domain.Entities;
using ScadaServer.Application.DTOs;
namespace ScadaServer.Application.Interfaces
{
    public interface IHistoricalRecordAppService
    {
        Task<HistoricalRecordDto> GetByIdAsync(long id);
        Task<List<HistoricalRecordDto>> GetListAsync();
        Task CreateAsync(HistoricalRecordDto dto);
        Task UpdateAsync(HistoricalRecordDto dto);
        Task DeleteAsync(long id);
    }
}

