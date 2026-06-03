using ScadaServer.Application.Interfaces;
using ScadaServer.Application.DTOs;
using ScadaServer.Domain.Entities;
using ScadaServer.Domain.Exceptions;
using ScadaServer.Domain.Enums;
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
                Key = entity.Key,
                AreaId = entity.AreaId,
                ModelId = entity.ModelId,
                Type = entity.Type,
                IpAddress = entity.IpAddress,
                Port = entity.Port,
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
                Key = entity.Key,
                AreaId = entity.AreaId,
                ModelId = entity.ModelId,
                Type = entity.Type,
                IpAddress = entity.IpAddress,
                Port = entity.Port,
                Status = entity.Status,
                CpuType = entity.CpuType,
                Rack = entity.Rack,
                Slot = entity.Slot,
                LastUpdated = entity.LastUpdated
            }).ToList();
        }

        public async Task<DeviceDto> CreateAsync(CreateDeviceDto dto)
        {
            // 1. 业务校验：Key 唯一性
            var existing = await _repository.GetListAsync(d => d.Key == dto.Key);
            if (existing.Any())
            {
                throw new BusinessException($"设备标识 '{dto.Key}' 已存在");
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

            // 3. 协议一致性检查：设备协议类型必须与模型定义的协议类型一致
            if (model.Type != dto.Type)
            {
                throw new BusinessException($"协议类型冲突。所选模型 '{model.Name}' 的类型为 {model.Type}，而当前设备配置的类型为 {dto.Type}。");
            }

            // 4. 协议特定校验
            if (dto.Type == "S7" && string.IsNullOrEmpty(dto.CpuType))
            {
                throw new BusinessException("西门子 S7 协议必须指定 CPU 类型");
            }

            var entity = new Device
            {
                Name = dto.Name,
                Key = dto.Key,
                AreaId = dto.AreaId,
                ModelId = dto.ModelId,
                Type = dto.Type,
                IpAddress = dto.IpAddress,
                Port = dto.Port,
                Status = dto.Status,
                CpuType = dto.CpuType,
                Rack = dto.Rack,
                Slot = dto.Slot,
                LastUpdated = DateTime.Now
            };
            await _repository.InsertAsync(entity);
            return await GetByIdAsync(entity.Id);
        }

        public async Task<DeviceDto> UpdateAsync(DeviceDto dto)
        {
            var entity = await _repository.GetByIdAsync(dto.Id);
            if (entity == null)
            {
                throw new BusinessException($"ID 为 {dto.Id} 的设备不存在");
            }

            // 1. 运行状态保护：在线设备禁止修改关键通信参数
            if (entity.Status == DeviceStatus.Online)
            {
                bool criticalChanged = entity.IpAddress != dto.IpAddress ||
                                     entity.Port != dto.Port ||
                                     entity.Type != dto.Type;

                if (criticalChanged)
                {
                    throw new BusinessException($"设备 '{entity.Name}' 处于在线运行状态，禁止修改 IP、端口或协议类型。请先停止设备。");
                }
            }

            // 2. 业务校验：Key 不能与其他设备重复
            var existing = await _repository.GetListAsync(d => d.Key == dto.Key && d.Id != dto.Id);
            if (existing.Any())
            {
                throw new BusinessException($"设备标识 '{dto.Key}' 已存在");
            }

            // 3. 存在性检查：校验区域和模型是否存在
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

            // 4. 协议一致性检查：设备协议类型必须与模型定义的协议类型一致
            if (model.Type != dto.Type)
            {
                throw new BusinessException($"协议类型冲突。所选模型 '{model.Name}' 的类型为 {model.Type}，而当前设备配置的类型为 {dto.Type}。");
            }

            entity.Name = dto.Name;
            entity.Key = dto.Key;
            entity.AreaId = dto.AreaId;
            entity.ModelId = dto.ModelId;
            entity.Type = dto.Type;
            entity.IpAddress = dto.IpAddress;
            entity.Port = dto.Port;
            entity.Status = dto.Status;
            entity.CpuType = dto.CpuType;
            entity.Rack = dto.Rack;
            entity.Slot = dto.Slot;
            entity.LastUpdated = DateTime.Now;

            await _repository.UpdateAsync(entity);
            return await GetByIdAsync(dto.Id);
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null) return;

            // 1. 安全校验：运行中的设备禁止删除
            if (entity.Status == DeviceStatus.Online)
            {
                throw new BusinessException($"无法删除设备 '{entity.Name}'，因为它当前处于在线状态。请先停止采集任务。");
            }

            // 2. 依赖检查：检查是否被对外接口引用
            var interfaces = await _interfaceRepository.GetListAsync(i => i.DeviceId == id);
            if (interfaces.Any())
            {
                throw new BusinessException($"无法删除设备 '{entity.Name}'，因为它已被配置到 {interfaces.Count} 个对外数据接口中。请先解除绑定。");
            }

            await using var transaction = await _uow.BeginTransactionAsync();
            try
            {
                // 删除级联数据
                await _sensorRepository.DeleteRangeAsync(s => s.DeviceId == id);
                await _triggerRepository.DeleteRangeAsync(t => t.DeviceId == id);
                await _realtimeDataRepository.DeleteRangeAsync(r => r.DeviceId == id);
                await _interfaceRepository.DeleteRangeAsync(i => i.DeviceId == id);

                // 删除设备
                await _repository.DeleteAsync(entity);

                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task UpdateDeviceConfigTxAsync(int deviceId, string newAddress) { }
    }
}
