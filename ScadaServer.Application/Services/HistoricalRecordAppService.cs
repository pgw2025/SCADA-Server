using ScadaServer.Application.Interfaces;
using ScadaServer.Application.DTOs;
using ScadaServer.Domain.Entities;
namespace ScadaServer.Application.Services
{
    public class HistoricalRecordAppService : IHistoricalRecordAppService
    {
        private readonly IHistoricalRecordRepository _repository;
        public HistoricalRecordAppService(IHistoricalRecordRepository repository) { _repository = repository; }

        public async Task<HistoricalRecordDto> GetByIdAsync(long id)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null) return null;
            return new HistoricalRecordDto
            {
                Id = entity.Id,
                DeviceId = entity.DeviceId,
                VariableKey = entity.VariableKey,
                Value = entity.Value,
                Timestamp = entity.Timestamp
            };
        }

        public async Task<List<HistoricalRecordDto>> GetListAsync()
        {
            var list = await _repository.GetListAsync();
            return list.Select(entity => new HistoricalRecordDto
            {
                Id = entity.Id,
                DeviceId = entity.DeviceId,
                VariableKey = entity.VariableKey,
                Value = entity.Value,
                Timestamp = entity.Timestamp
            }).ToList();
        }

        public async Task CreateAsync(HistoricalRecordDto dto)
        {
            var entity = new HistoricalRecord
            {
                DeviceId = dto.DeviceId,
                VariableKey = dto.VariableKey,
                Value = dto.Value,
                Timestamp = dto.Timestamp
            };
            await _repository.InsertAsync(entity);
        }

        public async Task UpdateAsync(HistoricalRecordDto dto)
        {
            var entity = await ((HistoricalRecordRepository)_repository).GetByIdAsync(dto.Id);
            if (entity != null)
            {
                entity.DeviceId = dto.DeviceId;
                entity.VariableKey = dto.VariableKey;
                entity.Value = dto.Value;
                entity.Timestamp = dto.Timestamp;
                await _repository.UpdateAsync(entity);
            }
        }

        public async Task DeleteAsync(long id)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity != null)
            {
                await _repository.DeleteAsync(entity);
            }
        }
    }
}

