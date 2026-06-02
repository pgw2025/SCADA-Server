using Microsoft.AspNetCore.Mvc;
using ScadaServer.Application.Interfaces;
using ScadaServer.Domain.Entities;
using ScadaServer.Application.DTOs;

namespace ScadaServer.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DataModelController : ControllerBase
    {
        private readonly IDataModelAppService _appService;
        private readonly IDataModelRepository _repo;

        public DataModelController(IDataModelAppService appService, IDataModelRepository repo)
        {
            _appService = appService;
            _repo = repo;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll() => Ok(await _repo.GetListAsync());

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id) => Ok(await _repo.GetByIdAsync(id));

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateDataModelDto dto)
        {
            var entity = new DataModel
            {
                Name = dto.Name,
                Description = dto.Description,
                Type = dto.Type
            };
            await _repo.InsertAsync(entity);
            return Ok(entity);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] CreateDataModelDto dto)
        {
            var entity = await _repo.GetByIdAsync(id);
            if (entity == null) return NotFound();

            entity.Name = dto.Name;
            entity.Description = dto.Description;
            entity.Type = dto.Type;

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
