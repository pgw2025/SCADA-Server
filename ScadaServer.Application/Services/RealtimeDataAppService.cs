using ScadaServer.Application.Interfaces;
using ScadaServer.Application.DTOs;
using ScadaServer.Domain.Entities;
namespace ScadaServer.Application.Services
{
    public class RealtimeDataAppService : IRealtimeDataAppService
    {
        private readonly IRealtimeDataRepository _repository;
        public RealtimeDataAppService(IRealtimeDataRepository repository) { _repository = repository; }

        public async Task<RealtimeDataDto> GetByIdAsync(int deviceId, string variableKey)
        {
            // For composite keys, we might need a custom method in repository or use Queryable
            // Assuming the repository handles it if we pass an anonymous object or array
            var entity = await _repository.GetByIdAsync(deviceId, variableKey);
            if (entity == null) return null;
            return new RealtimeDataDto
            {
                DeviceId = entity.DeviceId,
                VariableKey = entity.VariableKey,
                Value = entity.Value,
                Timestamp = entity.Timestamp
            };
        }

        public async Task<List<RealtimeDataDto>> GetListAsync()
        {
            var list = await _repository.GetListAsync();
            return list.Select(entity => new RealtimeDataDto
            {
                DeviceId = entity.DeviceId,
                VariableKey = entity.VariableKey,
                Value = entity.Value,
                Timestamp = entity.Timestamp
            }).ToList();
        }

        public async Task CreateAsync(RealtimeDataDto dto)
        {
            var entity = new RealtimeData
            {
                DeviceId = dto.DeviceId,
                VariableKey = dto.VariableKey,
                Value = dto.Value,
                Timestamp = dto.Timestamp
            };
            await _repository.InsertAsync(entity);
        }

        public async Task UpdateAsync(RealtimeDataDto dto)
        {
            var entity = await ((RealtimeDataRepository)_repository).GetByIdAsync(dto.DeviceId, dto.VariableKey);
            if (entity != null)
            {
                entity.Value = dto.Value;
                entity.Timestamp = dto.Timestamp;
                await _repository.UpdateAsync(entity);
            }
        }

        public async Task DeleteAsync(int deviceId, string variableKey)
        {
            var entity = await _repository.GetByIdAsync(deviceId, variableKey);
            if (entity != null)
            {
                await _repository.DeleteAsync(entity);
            }
        }
    }
}

