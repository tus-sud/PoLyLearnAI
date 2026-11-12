using Microsoft.AspNetCore.Mvc;

namespace PolyLearnAI.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HealthController : ControllerBase
{
    [HttpGet]
    public IActionResult Get() => Ok(new { status = "healthy", app = "PolyLearnAI", utc = DateTime.UtcNow });
}
