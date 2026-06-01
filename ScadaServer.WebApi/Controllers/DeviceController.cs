using Microsoft.AspNetCore.Mvc;
using ScadaServer.Application.Interfaces;
using ScadaServer.Domain.Entities;
using ScadaServer.Application.DTOs;

namespace ScadaServer.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DeviceController : ControllerBase
    {
        private readonly IDeviceAppService _deviceAppService;
        private readonly IDeviceRepository _deviceRepo;

        public DeviceController(IDeviceAppService deviceAppService, IDeviceRepository deviceRepo)
        {
            _deviceAppService = deviceAppService;
            _deviceRepo = deviceRepo;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll() => Ok(await _deviceRepo.GetListAsync());

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id) => Ok(await _deviceRepo.GetByIdAsync(id));

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateDeviceDto dto)
        {
            var device = new Device
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
            await _deviceRepo.InsertAsync(device);
            return Ok(device);
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] Device device)
        {
            await _deviceRepo.UpdateAsync(device);
            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var device = await _deviceRepo.GetByIdAsync(id);
            if (device != null) await _deviceRepo.DeleteAsync(device);
            return Ok();
        }

        // 业务编排接口：更新配置并记录日志（含事务）
        [HttpPost("{id}/update-config")]
        public async Task<IActionResult> UpdateConfig(int id, [FromBody] string newAddress)
        {
            await _deviceAppService.UpdateDeviceConfigTxAsync(id, newAddress);
            return Ok(new { Message = "Configuration updated with transaction." });
        }

        // 指令下发接口
        [HttpPost("{id}/control")]
        public async Task<IActionResult> SendCommand(int id, [FromBody] string command)
        {
            // Logic for sending command via protocol drivers
            return Ok(new { DeviceId = id, Status = "Command Sent", Payload = command });
        }
    }
}

