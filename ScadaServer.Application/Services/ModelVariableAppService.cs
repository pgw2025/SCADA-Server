using ScadaServer.Application.Interfaces;
using ScadaServer.Application.DTOs;
using ScadaServer.Domain.Entities;
namespace ScadaServer.Application.Services
{
    public class ModelVariableAppService : IModelVariableAppService
    {
        private readonly IModelVariableRepository _repository;
        public ModelVariableAppService(IModelVariableRepository repository) { _repository = repository; }

        public async Task<ModelVariableDto> GetByIdAsync(int id)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null) return null;
            return new ModelVariableDto
            {
                Id = entity.Id,
                ModelId = entity.ModelId,
                Key = entity.Key,
                Name = entity.Name,
                Type = entity.Type,
                DataType = entity.DataType,
                Unit = entity.Unit,
                Min = entity.Min,
                Max = entity.Max,
                Address = entity.Address,
                Description = entity.Description,
                IsStored = entity.IsStored,
                StoreMode = entity.StoreMode
            };
        }

        public async Task<List<ModelVariableDto>> GetListAsync()
        {
            var list = await _repository.GetListAsync();
            return list.Select(entity => new ModelVariableDto
            {
                Id = entity.Id,
                ModelId = entity.ModelId,
                Key = entity.Key,
                Name = entity.Name,
                Type = entity.Type,
                DataType = entity.DataType,
                Unit = entity.Unit,
                Min = entity.Min,
                Max = entity.Max,
                Address = entity.Address,
                Description = entity.Description,
                IsStored = entity.IsStored,
                StoreMode = entity.StoreMode
            }).ToList();
        }

        public async Task CreateAsync(ModelVariableDto dto)
        {
            var entity = new ModelVariable
            {
                ModelId = dto.ModelId,
                Key = dto.Key,
                Name = dto.Name,
                Type = dto.Type,
                DataType = dto.DataType,
                Unit = dto.Unit,
                Min = dto.Min,
                Max = dto.Max,
                Address = dto.Address,
                Description = dto.Description,
                IsStored = dto.IsStored,
                StoreMode = dto.StoreMode
            };
            await _repository.InsertAsync(entity);
        }

        public async Task UpdateAsync(ModelVariableDto dto)
        {
            var entity = await _repository.GetByIdAsync(dto.Id);
            if (entity != null)
            {
                entity.ModelId = dto.ModelId;
                entity.Key = dto.Key;
                entity.Name = dto.Name;
                entity.Type = dto.Type;
                entity.DataType = dto.DataType;
                entity.Unit = dto.Unit;
                entity.Min = dto.Min;
                entity.Max = dto.Max;
                entity.Address = dto.Address;
                entity.Description = dto.Description;
                entity.IsStored = dto.IsStored;
                entity.StoreMode = dto.StoreMode;
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

