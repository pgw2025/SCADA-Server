using ScadaServer.Application.Interfaces;
using ScadaServer.Application.DTOs;
using ScadaServer.Domain.Entities;
namespace ScadaServer.Application.Services
{
    public class DataModelAppService : IDataModelAppService
    {
        private readonly IDataModelRepository _repository;
        private readonly IModelVariableRepository _variableRepository;
        private readonly IVariableTriggerRepository _triggerRepository;
        private readonly IUnitOfWork _uow;

        public DataModelAppService(
            IDataModelRepository repository, 
            IModelVariableRepository variableRepository, 
            IVariableTriggerRepository triggerRepository,
            IUnitOfWork uow) 
        { 
            _repository = repository; 
            _variableRepository = variableRepository;
            _triggerRepository = triggerRepository;
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

        public async Task CreateAsync(DataModelDto dto)
        {
            var entity = new DataModel { Name = dto.Name, Description = dto.Description, Type = dto.Type };
            await _repository.InsertAsync(entity);
        }

        public async Task UpdateAsync(DataModelDto dto)
        {
            var entity = await _repository.GetByIdAsync(dto.Id);
            if (entity != null)
            {
                entity.Name = dto.Name;
                entity.Description = dto.Description;
                entity.Type = dto.Type;
                await _repository.UpdateAsync(entity);
            }
        }

        public async Task DeleteAsync(int id)
        {
            _uow.BeginTran();
            try
            {
                // 清理属于该模型的变量定义
                await _variableRepository.DeleteRangeAsync(v => v.ModelId == id);
                
                // 最后删除模型本身
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

