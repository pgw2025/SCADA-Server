using ScadaServer.Application.Interfaces;
using ScadaServer.Application.DTOs;
using ScadaServer.Domain.Entities;
using ScadaServer.Infrastructure.Repositories;

namespace ScadaServer.Application.Services
{
    public class ScadaPageAppService : IScadaPageAppService
    {
        private readonly ScadaPageRepository _repository;
        public ScadaPageAppService(ScadaPageRepository repository) { _repository = repository; }

        public async Task<ScadaPageDto> GetByIdAsync(int id)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null) return null;
            return new ScadaPageDto
            {
                Id = entity.Id,
                ProjectId = entity.ProjectId,
                Name = entity.Name,
                IsHome = entity.IsHome
            };
        }

        public async Task<List<ScadaPageDto>> GetListAsync()
        {
            var list = await _repository.GetListAsync();
            return list.Select(entity => new ScadaPageDto
            {
                Id = entity.Id,
                ProjectId = entity.ProjectId,
                Name = entity.Name,
                IsHome = entity.IsHome
            }).ToList();
        }

        public async Task CreateAsync(ScadaPageDto dto)
        {
            var entity = new ScadaPage
            {
                ProjectId = dto.ProjectId,
                Name = dto.Name,
                IsHome = dto.IsHome
            };
            await _repository.InsertAsync(entity);
        }

        public async Task UpdateAsync(ScadaPageDto dto)
        {
            var entity = await _repository.GetByIdAsync(dto.Id);
            if (entity != null)
            {
                entity.ProjectId = dto.ProjectId;
                entity.Name = dto.Name;
                entity.IsHome = dto.IsHome;
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
