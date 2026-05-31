using ScadaServer.Application.Interfaces;
using ScadaServer.Domain.Entities;

namespace ScadaServer.Application.Services
{
    public class DeviceAppService : IDeviceAppService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IDeviceRepository _deviceRepo;
        private readonly IRepository<ConfigLog> _logRepo;

        public DeviceAppService(
            IUnitOfWork unitOfWork, 
            IDeviceRepository deviceRepo, 
            IRepository<ConfigLog> logRepo)
        {
            _unitOfWork = unitOfWork;
            _deviceRepo = deviceRepo;
            _logRepo = logRepo;
        }

        public async Task UpdateDeviceConfigTxAsync(int deviceId, string newAddress)
        {
            _unitOfWork.BeginTran();

            try
            {
                var device = await _deviceRepo.GetByIdAsync(deviceId);
                if (device == null) throw new Exception("Device not found");
                
                device.Status = "ConfigUpdating";
                await _deviceRepo.UpdateAsync(device);

                var log = new ConfigLog { 
                    DeviceId = deviceId, 
                    Operator = "Admin", 
                    ChangeDesc = $"Address changed to: {newAddress}",
                    CreateTime = DateTime.Now 
                };
                await _logRepo.InsertAsync(log);

                await _unitOfWork.CommitTranAsync();
            }
            catch (Exception)
            {
                await _unitOfWork.RollbackTranAsync();
                throw;
            }
        }
    }
}
