using GuruPR.Application.Dapr;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GuruPR.Controllers;

[ApiController]
[Route("api/v1/Dapr")]
[Authorize]
public class DaprController : ControllerBase
{
    private readonly ILogger _logger;

    public DaprController(ILogger<DaprController> logger)
    {
        _logger = logger;
    }

    [HttpPost("/handle")]
    public async Task<ActionResult<string>> HandleInputBindingsAsync([FromBody] MessageEnvelope messageEnvelope)
    {
        // Treat incoming messages here ( Redirect based on operation-type ) ...
        await Task.CompletedTask;
        return Ok();
    }
}
