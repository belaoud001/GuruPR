using GuruPR.Application.Dapr;
using Microsoft.AspNetCore.Mvc;

namespace GuruPR.Controllers;

[ApiController]
public class Dapr : ControllerBase
{
    private readonly ILogger _logger;

    public Dapr(ILogger<Dapr> logger)
    {
        _logger = logger;
    }
    
    [HttpPost("/handle")]
    public async Task<ActionResult<string>> HandleInputBindingsAsync([FromBody] MessageEnvelope messageEnvelope)
    {
        // Treat incoming messages here ( Redirect based on operation-type ) ...
        
        return Ok();
    }
}
