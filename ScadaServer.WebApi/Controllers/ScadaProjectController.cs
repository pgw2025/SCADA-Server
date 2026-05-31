using Microsoft.AspNetCore.Mvc;
using ScadaServer.Application.Interfaces;
using ScadaServer.Domain.Entities;

namespace ScadaServer.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ScadaProjectController : ControllerBase
    {
        private readonly IScadaProjectAppService _appService;
        private readonly IScadaProjectRepository _repo;

        public ScadaProjectController(IScadaProjectAppService appService, IScadaProjectRepository repo)
        {
            _appService = appService;
            _repo = repo;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll() => Ok(await _repo.GetListAsync());

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id) => Ok(await _repo.GetByIdAsync(id));

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ScadaProject entity)
        {
            await _repo.InsertAsync(entity);
            return Ok(entity);
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] ScadaProject entity)
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
