using ScadaServer.Application.Interfaces;
using ScadaServer.Application.DTOs;
using ScadaServer.Domain.Entities;
namespace ScadaServer.Application.Services
{
    public class DeviceAppService : IDeviceAppService
    {
        private readonly IDeviceRepository _repository;
        private readonly ISensorRepository _sensorRepository;
        private readonly IVariableTriggerRepository _triggerRepository;
        private readonly IRealtimeDataRepository _realtimeDataRepository;
        private readonly IExposedInterfaceRepository _interfaceRepository;
        private readonly IUnitOfWork _uow;

        public DeviceAppService(
            IDeviceRepository repository, 
            ISensorRepository sensorRepository,
            IVariableTriggerRepository triggerRepository,
            IRealtimeDataRepository realtimeDataRepository,
            IExposedInterfaceRepository interfaceRepository,
            IUnitOfWork uow) 
        { 
            _repository = repository; 
            _sensorRepository = sensorRepository;
            _triggerRepository = triggerRepository;
            _realtimeDataRepository = realtimeDataRepository;
            _interfaceRepository = interfaceRepository;
            _uow = uow;
        }

        public async Task<DeviceDto> GetByIdAsync(int id)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null) return null;
            return new DeviceDto
            {
                Id = entity.Id,
                Name = entity.Name,
                Code = entity.Code,
                AreaId = entity.AreaId,
                Type = entity.Type,
                IpAddress = entity.IpAddress,
                Port = entity.Port,
                Topic = entity.Topic,
                Status = entity.Status,
                CpuType = entity.CpuType,
                Rack = entity.Rack,
                Slot = entity.Slot,
                LastUpdated = entity.LastUpdated
            };
        }

        public async Task<List<DeviceDto>> GetListAsync()
        {
            var list = await _repository.GetListAsync();
            return list.Select(entity => new DeviceDto
            {
                Id = entity.Id,
                Name = entity.Name,
                Code = entity.Code,
                AreaId = entity.AreaId,
                Type = entity.Type,
                IpAddress = entity.IpAddress,
                Port = entity.Port,
                Topic = entity.Topic,
                Status = entity.Status,
                CpuType = entity.CpuType,
                Rack = entity.Rack,
                Slot = entity.Slot,
                LastUpdated = entity.LastUpdated
            }).ToList();
        }

        public async Task CreateAsync(DeviceDto dto)
        {
            var entity = new Device
            {
                Name = dto.Name,
                Code = dto.Code,
                AreaId = dto.AreaId,
                ModelId = dto.ModelId,
                Type = dto.Type,
                IpAddress = dto.IpAddress,
                Port = dto.Port,
                Topic = dto.Topic,
                Status = dto.Status,
                CpuType = dto.CpuType,
                Rack = dto.Rack,
                Slot = dto.Slot,
                LastUpdated = DateTime.Now
            };
            await _repository.InsertAsync(entity);
        }

        public async Task UpdateAsync(DeviceDto dto)
        {
            var entity = await _repository.GetByIdAsync(dto.Id);
            if (entity != null)
            {
                entity.Name = dto.Name;
                entity.Code = dto.Code;
                entity.AreaId = dto.AreaId;
                entity.ModelId = dto.ModelId;
                entity.Type = dto.Type;
                entity.IpAddress = dto.IpAddress;
                entity.Port = dto.Port;
                entity.Topic = dto.Topic;
                entity.Status = dto.Status;
                entity.CpuType = dto.CpuType;
                entity.Rack = dto.Rack;
                entity.Slot = dto.Slot;
                entity.LastUpdated = DateTime.Now;
                await _repository.UpdateAsync(entity);
            }
        }

        public async Task DeleteAsync(int id)
        {
            _uow.BeginTran();
            try
            {
                // 删除级联数据
                await _sensorRepository.DeleteRangeAsync(s => s.DeviceId == id);
                await _triggerRepository.DeleteRangeAsync(t => t.DeviceId == id);
                await _realtimeDataRepository.DeleteRangeAsync(r => r.DeviceId == id);
                await _interfaceRepository.DeleteRangeAsync(i => i.DeviceId == id);
                
                // 删除设备
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
        
        public async Task UpdateDeviceConfigTxAsync(int deviceId, string newAddress) { }
    }
}

