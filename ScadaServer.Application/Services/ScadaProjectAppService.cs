using ScadaServer.Application.Interfaces;
using ScadaServer.Application.DTOs;
using ScadaServer.Domain.Entities;
using ScadaServer.Infrastructure.Repositories;

namespace ScadaServer.Application.Services
{
    public class ScadaProjectAppService : IScadaProjectAppService
    {
        private readonly ScadaProjectRepository _repository;
        public ScadaProjectAppService(ScadaProjectRepository repository) { _repository = repository; }

        public async Task<ScadaProjectDto> GetByIdAsync(int id)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null) return null;
            return new ScadaProjectDto
            {
                Id = entity.Id,
                Name = entity.Name,
                Description = entity.Description,
                CreatedAt = entity.CreatedAt
            };
        }

        public async Task<List<ScadaProjectDto>> GetListAsync()
        {
            var list = await _repository.GetListAsync();
            return list.Select(entity => new ScadaProjectDto
            {
                Id = entity.Id,
                Name = entity.Name,
                Description = entity.Description,
                CreatedAt = entity.CreatedAt
            }).ToList();
        }

        public async Task CreateAsync(ScadaProjectDto dto)
        {
            var entity = new ScadaProject
            {
                Name = dto.Name,
                Description = dto.Description,
                CreatedAt = dto.CreatedAt
            };
            await _repository.InsertAsync(entity);
        }

        public async Task UpdateAsync(ScadaProjectDto dto)
        {
            var entity = await _repository.GetByIdAsync(dto.Id);
            if (entity != null)
            {
                entity.Name = dto.Name;
                entity.Description = dto.Description;
                entity.CreatedAt = dto.CreatedAt;
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
