using ScadaServer.Application.Interfaces;
using ScadaServer.Application.DTOs;
using ScadaServer.Domain.Entities;
using ScadaServer.Domain.Exceptions;
namespace ScadaServer.Application.Services
{
    public class DeviceAppService : IDeviceAppService
    {
        private readonly IDeviceRepository _repository;
        private readonly ISensorRepository _sensorRepository;
        private readonly IVariableTriggerRepository _triggerRepository;
        private readonly IRealtimeDataRepository _realtimeDataRepository;
        private readonly IExposedInterfaceRepository _interfaceRepository;
        private readonly IAreaRepository _areaRepository;
        private readonly IDataModelRepository _modelRepository;
        private readonly IUnitOfWork _uow;

        public DeviceAppService(
            IDeviceRepository repository, 
            ISensorRepository sensorRepository,
            IVariableTriggerRepository triggerRepository,
            IRealtimeDataRepository realtimeDataRepository,
            IExposedInterfaceRepository interfaceRepository,
            IAreaRepository areaRepository,
            IDataModelRepository modelRepository,
            IUnitOfWork uow) 
        { 
            _repository = repository; 
            _sensorRepository = sensorRepository;
            _triggerRepository = triggerRepository;
            _realtimeDataRepository = realtimeDataRepository;
            _interfaceRepository = interfaceRepository;
            _areaRepository = areaRepository;
            _modelRepository = modelRepository;
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
                ModelId = entity.ModelId,
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
                ModelId = entity.ModelId,
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

        public async Task<DeviceDto> CreateAsync(CreateDeviceDto dto)
        {
            // 1. 业务校验：Code 唯一性
            var existing = await _repository.GetListAsync(d => d.Code == dto.Code);
            if (existing.Any())
            {
                throw new BusinessException($"设备编码 '{dto.Code}' 已存在");
            }

            // 2. 存在性检查：校验区域和模型是否存在
            var area = await _areaRepository.GetByIdAsync(dto.AreaId);
            if (area == null)
            {
                throw new BusinessException($"ID 为 {dto.AreaId} 的区域不存在");
            }

            var model = await _modelRepository.GetByIdAsync(dto.ModelId);
            if (model == null)
            {
                throw new BusinessException($"ID 为 {dto.ModelId} 的变量模型不存在");
            }

            // 3. 协议特定校验
            if (dto.Type == "S7" && string.IsNullOrEmpty(dto.CpuType))
            {
                throw new BusinessException("西门子 S7 协议必须指定 CPU 类型");
            }

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
            return await GetByIdAsync(entity.Id);
        }

        public async Task<DeviceDto> UpdateAsync(int id, CreateDeviceDto dto)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null)
            {
                throw new BusinessException($"ID 为 {id} 的设备不存在");
            }

            // 1. 业务校验：Code 不能与其他设备重复
            var existing = await _repository.GetListAsync(d => d.Code == dto.Code && d.Id != id);
            if (existing.Any())
            {
                throw new BusinessException($"设备编码 '{dto.Code}' 已存在");
            }

            // 2. 存在性检查：校验区域和模型是否存在
            var area = await _areaRepository.GetByIdAsync(dto.AreaId);
            if (area == null)
            {
                throw new BusinessException($"ID 为 {dto.AreaId} 的区域不存在");
            }

            var model = await _modelRepository.GetByIdAsync(dto.ModelId);
            if (model == null)
            {
                throw new BusinessException($"ID 为 {dto.ModelId} 的变量模型不存在");
            }

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
            return await GetByIdAsync(id);
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

