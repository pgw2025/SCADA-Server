using ScadaServer.Application.Interfaces;
using ScadaServer.Application.DTOs;
using ScadaServer.Domain.Entities;
namespace ScadaServer.Application.Services
{
    public class AreaAppService : IAreaAppService
    {
        private readonly IAreaRepository _repository;
        private readonly IDeviceRepository _deviceRepository;
        private readonly IUnitOfWork _uow;

        public AreaAppService(IAreaRepository repository, IDeviceRepository deviceRepository, IUnitOfWork uow) 
        { 
            _repository = repository; 
            _deviceRepository = deviceRepository;
            _uow = uow;
        }

        public async Task<AreaDto> GetByIdAsync(int id)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null) return null;
            return new AreaDto { Id = entity.Id, Name = entity.Name, Description = entity.Description };
        }

        public async Task<List<AreaDto>> GetListAsync()
        {
            var list = await _repository.GetListAsync();
            return list.Select(entity => new AreaDto { Id = entity.Id, Name = entity.Name, Description = entity.Description }).ToList();
        }

        public async Task CreateAsync(AreaDto dto)
        {
            var entity = new Area { Name = dto.Name, Description = dto.Description };
            await _repository.InsertAsync(entity);
        }

        public async Task UpdateAsync(AreaDto dto)
        {
            var entity = await _repository.GetByIdAsync(dto.Id);
            if (entity != null)
            {
                entity.Name = dto.Name;
                entity.Description = dto.Description;
                await _repository.UpdateAsync(entity);
            }
        }

        public async Task DeleteAsync(int id)
        {
            _uow.BeginTran();
            try
            {
                // 删除区域下所有设备
                await _deviceRepository.DeleteRangeAsync(d => d.AreaId == id);
                
                // 删除区域
                var entity = await _repository.GetByIdAsync(id);
                if (entity != null) await _repository.DeleteAsync(entity);

                await _uow.CommitTranAsync();
            }
            catch
            {
                await _uow.RollbackTranAsync();
                throw;
            }
        }
    }
}

