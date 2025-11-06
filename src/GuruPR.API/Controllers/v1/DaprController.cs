using Asp.Versioning;

using GuruPR.Application.Dapr;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GuruPR.Controllers.v1;

[Authorize]
[ApiController]
[ApiVersion(1.0)]
[Route("api/v{version:apiVersion}/Dapr")]
public class DaprController : ControllerBase
{
    private readonly ILogger _logger;

    public DaprController(ILogger<DaprController> logger)
    {
        _logger = logger;
    }

    [HttpPost("handle")]
    public async Task<ActionResult<string>> HandleInputBindingsAsync([FromBody] MessageEnvelope messageEnvelope)
    {
        // Treat incoming messages here ( Redirect based on operation-type ) ...
        await Task.CompletedTask;
        return Ok();
    }
}
