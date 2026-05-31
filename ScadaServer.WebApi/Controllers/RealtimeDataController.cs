using Microsoft.AspNetCore.Mvc;
using ScadaServer.Application.Interfaces;
using ScadaServer.Domain.Entities;

namespace ScadaServer.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RealtimeDataController : ControllerBase
    {
        private readonly IRealtimeDataAppService _appService;
        private readonly IRealtimeDataRepository _repo;

        public RealtimeDataController(IRealtimeDataAppService appService, IRealtimeDataRepository repo)
        {
            _appService = appService;
            _repo = repo;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll() => Ok(await _repo.GetListAsync());

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id) => Ok(await _repo.GetByIdAsync(id));

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] RealtimeData entity)
        {
            await _repo.InsertAsync(entity);
            return Ok(entity);
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] RealtimeData entity)
        {
            await _repo.UpdateAsync(entity);
            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var entity = await _repo.GetByIdAsync(id);
            if (entity != null) await _repo.DeleteAsync(entity);
            return Ok();
        }
    }
}
