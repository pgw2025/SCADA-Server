using ScadaServer.Domain.Entities;
using ScadaServer.Application.DTOs;
namespace ScadaServer.Application.Interfaces
{
    public interface IRealtimeDataAppService
    {
        Task<RealtimeDataDto> GetByIdAsync(int deviceId, string variableKey);
        Task<List<RealtimeDataDto>> GetListAsync();
        Task CreateAsync(RealtimeDataDto dto);
        Task UpdateAsync(RealtimeDataDto dto);
        Task DeleteAsync(int deviceId, string variableKey);
    }
}

