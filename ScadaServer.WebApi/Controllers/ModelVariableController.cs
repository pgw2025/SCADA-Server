using Microsoft.AspNetCore.Mvc;
using ScadaServer.Application.DTOs;
using ScadaServer.Application.Interfaces;
using ScadaServer.Domain.Entities;

namespace ScadaServer.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ModelVariableController : ControllerBase
    {
        private readonly IModelVariableRepository _repo;

        public ModelVariableController(IModelVariableRepository repo)
        {
            _repo = repo;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var list = await _repo.GetListAsync();
            return Ok(list.Select(MapToDto));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var entity = await _repo.GetByIdAsync(id);
            return entity == null ? NotFound() : Ok(MapToDto(entity));
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ModelVariableDto dto)
        {
            var entity = MapToEntity(dto);
            await _repo.InsertAsync(entity);
            return Ok(MapToDto(entity));
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] ModelVariableDto dto)
        {
            var entity = await _repo.GetByIdAsync(dto.Id);
            if (entity == null) return NotFound();

            MapToEntity(dto, entity);
            await _repo.UpdateAsync(entity);
            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var entity = await _repo.GetByIdAsync(id);
            if (entity == null) return NotFound();
            await _repo.DeleteAsync(entity);
            return Ok();
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
            ExtensionData = entity.ExtensionData
        };

        private static ModelVariable MapToEntity(ModelVariableDto dto) => MapToEntity(dto, new ModelVariable());

        private static ModelVariable MapToEntity(ModelVariableDto dto, ModelVariable entity)
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
            entity.UpdateMode = dto.UpdateMode;
            entity.PollingIntervalMs = dto.PollingIntervalMs;
            entity.ExtensionData = dto.ExtensionData;
            return entity;
        }
    }
}
