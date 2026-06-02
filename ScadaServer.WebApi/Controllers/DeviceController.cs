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

        public DeviceController(IDeviceAppService deviceAppService)
        {
            _deviceAppService = deviceAppService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll() => Ok(await _deviceAppService.GetListAsync());

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id) => Ok(await _deviceAppService.GetByIdAsync(id));

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateDeviceDto dto)
        {
            var result = await _deviceAppService.CreateAsync(dto);
            return Ok(result);
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] DeviceDto dto)
        {
            var result = await _deviceAppService.UpdateAsync(dto);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _deviceAppService.DeleteAsync(id);
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

