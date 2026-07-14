using Microsoft.AspNetCore.Mvc;

namespace Fcg.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class HealthController : ControllerBase
{
    [HttpGet]
    public IActionResult Get()
    {
        return Ok(new
        {
            status = "ok",
            service = "FIAP Cloud Games API",
            utcNow = DateTime.UtcNow
        });
    }
}
