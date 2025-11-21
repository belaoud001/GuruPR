using Asp.Versioning;

using Microsoft.AspNetCore.Mvc;

namespace GuruPR.Controllers.v1;

[ApiController]
[ApiVersion(1.0)]
[Route("api/v{version:apiVersion}/health")]
public class HealthController : ControllerBase
{
    [HttpGet]
    public IActionResult Health()
    {
        return Ok("Healthy");
    }
}
