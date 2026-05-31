using ScadaServer.Application.Interfaces;
using ScadaServer.Application.DTOs;
using ScadaServer.Domain.Entities;
using ScadaServer.Infrastructure.Repositories;

namespace ScadaServer.Application.Services
{
    public class DataModelAppService : IDataModelAppService
    {
        private readonly DataModelRepository _repository;
        public DataModelAppService(DataModelRepository repository) { _repository = repository; }

        public async Task<DataModelDto> GetByIdAsync(int id)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null) return null;
            return new DataModelDto
            {
                Id = entity.Id,
                Name = entity.Name,
                Description = entity.Description,
                Type = entity.Type
            };
        }

        public async Task<List<DataModelDto>> GetListAsync()
        {
            var list = await _repository.GetListAsync();
            return list.Select(entity => new DataModelDto
            {
                Id = entity.Id,
                Name = entity.Name,
                Description = entity.Description,
                Type = entity.Type
            }).ToList();
        }

        public async Task CreateAsync(DataModelDto dto)
        {
            var entity = new DataModel
            {
                Name = dto.Name,
                Description = dto.Description,
                Type = dto.Type
            };
            await _repository.InsertAsync(entity);
        }

        public async Task UpdateAsync(DataModelDto dto)
        {
            var entity = await _repository.GetByIdAsync(dto.Id);
            if (entity != null)
            {
                entity.Name = dto.Name;
                entity.Description = dto.Description;
                entity.Type = dto.Type;
                await _repository.UpdateAsync(entity);
            }
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity != null)
            {
                await _repository.DeleteAsync(entity);
            }
        }
    }
}
