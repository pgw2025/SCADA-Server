using Microsoft.AspNetCore.Mvc;
using ScadaServer.Application.Interfaces;
using ScadaServer.Domain.Entities;

namespace ScadaServer.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SensorController : ControllerBase
    {
        private readonly IRepository<Sensor> _sensorRepo;

        public SensorController(IRepository<Sensor> sensorRepo)
        {
            _sensorRepo = sensorRepo;
        }

        [HttpGet("device/{deviceId}")]
        public async Task<IActionResult> GetByDevice(int deviceId)
        {
            var list = await _sensorRepo.GetListAsync();
            return Ok(list.Where(s => s.DeviceId == deviceId));
        }

        [HttpPost]
        public async Task<IActionResult> AddSensor([FromBody] Sensor sensor)
        {
            await _sensorRepo.InsertAsync(sensor);
            return Ok(sensor);
        }
    }
}
