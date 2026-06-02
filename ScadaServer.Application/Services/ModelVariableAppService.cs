using ScadaServer.Application.Interfaces;
using ScadaServer.Application.DTOs;
using ScadaServer.Domain.Entities;
using ScadaServer.Domain.Exceptions;

namespace ScadaServer.Application.Services
{
    public class ModelVariableAppService : IModelVariableAppService
    {
        private readonly IModelVariableRepository _repository;
        private readonly IDataModelRepository _modelRepository;

        public ModelVariableAppService(IModelVariableRepository repository, IDataModelRepository modelRepository) 
        { 
            _repository = repository; 
            _modelRepository = modelRepository;
        }

        public async Task<ModelVariableDto> GetByIdAsync(int id)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null) return null;
            return MapToDto(entity);
        }

        public async Task<List<ModelVariableDto>> GetListAsync()
        {
            var list = await _repository.GetListAsync();
            return list.Select(MapToDto).ToList();
        }

        public async Task<ModelVariableDto> CreateAsync(ModelVariableDto dto)
        {
            // 0. 规范化
            dto.Key = dto.Key?.Trim();
            dto.Name = dto.Name?.Trim();
            dto.Address = dto.Address?.Trim();

            // 1. 存在性检查：模型必须存在
            var modelExists = await _modelRepository.AnyAsync(m => m.Id == dto.ModelId);
            if (!modelExists)
            {
                throw new BusinessException($"ID 为 {dto.ModelId} 的数据模型不存在");
            }

            // 2. 业务校验：在同一个模型下 Key 必须唯一
            var keyExists = await _repository.AnyAsync(v => v.ModelId == dto.ModelId && v.Key == dto.Key);
            if (keyExists)
            {
                throw new BusinessException($"模型内已存在标识为 '{dto.Key}' 的变量");
            }

            var entity = MapToEntity(dto);
            await _repository.InsertAsync(entity);
            
            // 注意：仓储应支持 ID 回填，否则这里返回的 DTO ID 会是 0
            dto.Id = entity.Id; 
            return dto;
        }

        public async Task<ModelVariableDto> UpdateAsync(ModelVariableDto dto)
        {
            // 0. 规范化
            dto.Key = dto.Key?.Trim();
            dto.Name = dto.Name?.Trim();
            dto.Address = dto.Address?.Trim();

            var entity = await _repository.GetByIdAsync(dto.Id);
            if (entity == null)
            {
                throw new BusinessException($"ID 为 {dto.Id} 的变量定义不存在");
            }

            // 1. 业务校验：Key 查重（排除自身）
            var keyExists = await _repository.AnyAsync(v => v.ModelId == dto.ModelId && v.Key == dto.Key && v.Id != dto.Id);
            if (keyExists)
            {
                throw new BusinessException($"模型内已存在标识为 '{dto.Key}' 的变量");
            }

            MapToEntity(dto, entity);
            await _repository.UpdateAsync(entity);
            
            return dto;
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity != null)
            {
                await _repository.DeleteAsync(entity);
            }
        }

        private static ModelVariableDto MapToDto(ModelVariable entity) => new()
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
            StoreMode = entity.StoreMode,
            UpdateMode = entity.UpdateMode,
            PollingIntervalMs = entity.PollingIntervalMs,
            BitOffset = entity.BitOffset,
            ScaleSlope = entity.ScaleSlope,
            ScaleOffset = entity.ScaleOffset,
            DeadBand = entity.DeadBand,
            IsReadOnly = entity.IsReadOnly,
            ExtensionData = entity.ExtensionData
        };

        private static ModelVariable MapToEntity(ModelVariableDto dto, ModelVariable? entity = null)
        {
            entity ??= new ModelVariable();
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
            entity.UpdateMode = dto.UpdateMode;
            entity.PollingIntervalMs = dto.PollingIntervalMs;
            entity.BitOffset = dto.BitOffset;
            entity.ScaleSlope = dto.ScaleSlope;
            entity.ScaleOffset = dto.ScaleOffset;
            entity.DeadBand = dto.DeadBand;
            entity.IsReadOnly = dto.IsReadOnly;
            entity.ExtensionData = dto.ExtensionData;
            return entity;
        }
    }
}

