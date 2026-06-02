using ScadaServer.Application.Interfaces;
using ScadaServer.Application.DTOs;
using ScadaServer.Domain.Entities;
using ScadaServer.Domain.Exceptions;

namespace ScadaServer.Application.Services
{
    public class DataModelAppService : IDataModelAppService
    {
        private readonly IDataModelRepository _repository;
        private readonly IModelVariableRepository _variableRepository;
        private readonly IDeviceRepository _deviceRepository;
        private readonly IUnitOfWork _uow;

        public DataModelAppService(
            IDataModelRepository repository, 
            IModelVariableRepository variableRepository, 
            IDeviceRepository deviceRepository,
            IUnitOfWork uow) 
        { 
            _repository = repository; 
            _variableRepository = variableRepository;
            _deviceRepository = deviceRepository;
            _uow = uow;
        }

        public async Task<DataModelDto> GetByIdAsync(int id)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null) return null;
            return new DataModelDto { Id = entity.Id, Name = entity.Name, Description = entity.Description, Type = entity.Type };
        }

        public async Task<List<DataModelDto>> GetListAsync()
        {
            var list = await _repository.GetListAsync();
            return list.Select(entity => new DataModelDto { Id = entity.Id, Name = entity.Name, Description = entity.Description, Type = entity.Type }).ToList();
        }

        public async Task<DataModelDto> CreateAsync(CreateDataModelDto dto)
        {
            // 1. 业务校验：名称唯一性
            var existing = await _repository.GetListAsync(m => m.Name == dto.Name);
            if (existing.Any())
            {
                throw new BusinessException($"数据模型名称 '{dto.Name}' 已存在");
            }

            var entity = new DataModel { Name = dto.Name, Description = dto.Description, Type = dto.Type };
            await _repository.InsertAsync(entity);
            
            return new DataModelDto { Id = entity.Id, Name = entity.Name, Description = entity.Description, Type = entity.Type };
        }

        public async Task<DataModelDto> UpdateAsync(int id, CreateDataModelDto dto)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null)
            {
                throw new BusinessException($"ID 为 {id} 的数据模型不存在");
            }

            // 1. 业务校验：名称不能与其他模型重复
            var existing = await _repository.GetListAsync(m => m.Name == dto.Name && m.Id != id);
            if (existing.Any())
            {
                throw new BusinessException($"数据模型名称 '{dto.Name}' 已存在");
            }

            entity.Name = dto.Name;
            entity.Description = dto.Description;
            entity.Type = dto.Type;
            await _repository.UpdateAsync(entity);

            return new DataModelDto { Id = entity.Id, Name = entity.Name, Description = entity.Description, Type = entity.Type };
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null) return;

            // 1. 安全检查：如果已有设备引用此模型，禁止删除
            var hasDevices = await _deviceRepository.AnyAsync(d => d.ModelId == id);
            if (hasDevices)
            {
                throw new BusinessException($"无法删除模型 '{entity.Name}'，因为已有设备正在使用此模型。请先删除相关设备。");
            }

            _uow.BeginTran();
            try
            {
                // 清理属于该模型的变量定义
                await _variableRepository.DeleteRangeAsync(v => v.ModelId == id);
                
                // 最后删除模型本身
                await _repository.DeleteAsync(entity);

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

