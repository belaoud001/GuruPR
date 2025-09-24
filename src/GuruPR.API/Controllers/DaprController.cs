using GuruPR.Application.Dapr;
using GuruPR.Application.Services.OAuth;
using Microsoft.AspNetCore.Mvc;

namespace GuruPR.Controllers;

[ApiController]
[Route("/Dapr")]
public class DaprController : ControllerBase
{
    private readonly ILogger _logger;
    private readonly ProviderManagementService providerManagementService;

    public DaprController(ILogger<DaprController> logger, ProviderManagementService providerManagementService)
    {
        _logger = logger;
        this.providerManagementService = providerManagementService;
    }
    
    [HttpPost("/handle")]
    public async Task<ActionResult<string>> HandleInputBindingsAsync([FromBody] MessageEnvelope messageEnvelope)
    {
        // Treat incoming messages here ( Redirect based on operation-type ) ...
        
        return Ok();
    }

    [HttpGet]
    public async Task<ActionResult<string>> TestAsync()
    {
        var result = await providerManagementService.TestDatabaseConnectionAsync();

        return Ok(result.ToString());
    }
}
