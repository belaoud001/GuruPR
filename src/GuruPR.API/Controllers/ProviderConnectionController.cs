using Microsoft.AspNetCore.Mvc;

using GuruPR.Application.Interfaces.Application;
using GuruPR.Application.Dtos.OAuth.ProviderConnection;

namespace GuruPR.Controllers;

[ApiController]
[Route("api/v1/providers/{providerId}/provider-connections")]
public class ProviderConnectionController : ControllerBase
{
    private readonly ILogger<ProviderConnectionController> _logger;
    private readonly IProviderConnectionService _providerConnectionService;

    public ProviderConnectionController(ILogger<ProviderConnectionController> logger, IProviderConnectionService providerConnectionService)
    {
        _logger = logger;
        _providerConnectionService = providerConnectionService;
    }


    [HttpGet]
    public async Task<IActionResult> GetConnectionsByProviderAsync(string providerId)
    {
        var connections = await _providerConnectionService.GetConnectionsByProviderAsync(providerId);

        return Ok(connections);
    }

    [HttpGet("{providerConnectionId}", Name = "GetProviderConnectionById")]
    public async Task<IActionResult> GetProviderConnectionByIdAsync(string providerId, string providerConnectionId)
    {
        var providerConnection = await _providerConnectionService.GetProviderConnectionByIdAsync(providerId, providerConnectionId);

        return Ok(providerConnection);
    }

    [HttpPost]
    public async Task<IActionResult> AddProviderConnectionToProviderAsync(string providerId, [FromBody] CreateProviderConnectionRequest createProviderConnectionRequest)
    {
        var providerConnection = await _providerConnectionService.AddProviderConnectionToProviderAsync(providerId, createProviderConnectionRequest);

        return CreatedAtRoute("GetProviderConnectionById", 
                              new { providerId = providerId, providerConnectionId = providerConnection.Id }, 
                              providerConnection);
    }

    [HttpPut("{providerConnectionId}")]
    public async Task<IActionResult> UpdateProviderConnectionAsync(string providerId, string providerConnectionId, [FromBody] UpdateProviderConnectionRequest updateProviderConnectionRequest)
    {
        // Note: Update functionality is not implemented in the service layer as per the current design.
        // This endpoint is a placeholder for future implementation.
        return StatusCode(501, "Update functionality is not implemented.");
    }

    [HttpDelete("{providerConnectionId}")]
    public async Task<IActionResult> DeleteProviderConnectionAsync(string providerId, string providerConnectionId)
    {
        var result = await _providerConnectionService.DeleteProviderConnectionAsync(providerId, providerConnectionId);

        return result ? NoContent() : NotFound();
    }
}
