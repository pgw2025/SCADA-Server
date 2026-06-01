using ScadaServer.Application.Interfaces;
using ScadaServer.Application.DTOs;
using ScadaServer.Domain.Entities;
namespace ScadaServer.Application.Services
{
    public class SystemUserAppService : ISystemUserAppService
    {
        private readonly ISystemUserRepository _repository;
        public SystemUserAppService(ISystemUserRepository repository) { _repository = repository; }

        public async Task<SystemUserDto> GetByIdAsync(int id)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null) return null;
            return new SystemUserDto
            {
                Id = entity.Id,
                Username = entity.Username,
                Role = entity.Role,
                Status = entity.Status
            };
        }

        public async Task<List<SystemUserDto>> GetListAsync()
        {
            var list = await _repository.GetListAsync();
            return list.Select(entity => new SystemUserDto
            {
                Id = entity.Id,
                Username = entity.Username,
                Role = entity.Role,
                Status = entity.Status
            }).ToList();
        }

        public async Task CreateAsync(SystemUserDto dto)
        {
            var entity = new SystemUser
            {
                Username = dto.Username,
                Role = dto.Role,
                Status = dto.Status,
                PasswordHash = "DEFAULT_HASH" // In a real app, this would be handled properly
            };
            await _repository.InsertAsync(entity);
        }

        public async Task UpdateAsync(SystemUserDto dto)
        {
            var entity = await _repository.GetByIdAsync(dto.Id);
            if (entity != null)
            {
                entity.Username = dto.Username;
                entity.Role = dto.Role;
                entity.Status = dto.Status;
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

