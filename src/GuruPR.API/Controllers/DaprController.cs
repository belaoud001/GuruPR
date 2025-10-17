using Microsoft.AspNetCore.Mvc;

using GuruPR.Application.Dapr;

namespace GuruPR.Controllers;

[ApiController]
[Route("/Dapr")]
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
