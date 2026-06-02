using Microsoft.AspNetCore.Mvc;
using ScadaServer.Application.DTOs;
using ScadaServer.Infrastructure.Services;

namespace ScadaServer.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SystemController : ControllerBase
{
    private readonly SystemMonitorService _monitorService;

    public SystemController(SystemMonitorService monitorService)
    {
        _monitorService = monitorService;
    }

    [HttpGet("status")]
    public ActionResult<SystemStatusDto> GetStatus()
    {
        return Ok(_monitorService.CurrentStatus);
    }
}
