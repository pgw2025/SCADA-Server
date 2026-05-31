using Microsoft.AspNetCore.Mvc;
using ScadaServer.Application.Interfaces;
using ScadaServer.Domain.Entities;

namespace ScadaServer.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AlarmRuleController : ControllerBase
    {
        private readonly IRepository<AlarmRule> _ruleRepo;

        public AlarmRuleController(IRepository<AlarmRule> ruleRepo)
        {
            _ruleRepo = ruleRepo;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll() => Ok(await _ruleRepo.GetListAsync());

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] AlarmRule rule)
        {
            await _ruleRepo.InsertAsync(rule);
            return Ok(rule);
        }

        [HttpPut("{id}/toggle")]
        public async Task<IActionResult> Toggle(int id, [FromBody] bool enabled)
        {
            var rule = await _ruleRepo.GetByIdAsync(id);
            if (rule != null)
            {
                rule.IsEnabled = enabled;
                await _ruleRepo.UpdateAsync(rule);
            }
            return Ok();
        }
    }
}
