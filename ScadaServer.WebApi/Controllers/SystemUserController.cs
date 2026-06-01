using Microsoft.AspNetCore.Mvc;
using ScadaServer.Application.Interfaces;
using ScadaServer.Domain.Entities;
using ScadaServer.Application.DTOs;
using Microsoft.AspNetCore.Identity;

namespace ScadaServer.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SystemUserController : ControllerBase
    {
        private readonly ISystemUserAppService _appService;
        private readonly ISystemUserRepository _repo;

        public SystemUserController(ISystemUserAppService appService, ISystemUserRepository repo)
        {
            _appService = appService;
            _repo = repo;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll() => Ok(await _repo.GetListAsync());

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id) => Ok(await _repo.GetByIdAsync(id));

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateUserDto dto)
        {
            var passwordHasher = new PasswordHasher<SystemUser>();
            var user = new SystemUser
            {
                Username = dto.Username,
                Role = dto.Role,
                Status = dto.Status,
                PasswordHash = passwordHasher.HashPassword(new SystemUser(), dto.Password)
            };
            
            await _repo.InsertAsync(user);
            return Ok(new { Success = true, Message = "User created successfully" });
        }


        [HttpPut]
        public async Task<IActionResult> Update([FromBody] SystemUser entity)
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
