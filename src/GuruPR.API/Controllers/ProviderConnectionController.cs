using AutoMapper;

using GuruPR.Application.Dtos.OAuth.ProviderConnection;
using GuruPR.Application.Interfaces.Application;

using Microsoft.AspNetCore.Mvc;

namespace GuruPR.Controllers;

[ApiController]
[Route("api/v1/providers/{providerId}/provider-connections")]
public class ProviderConnectionController : ControllerBase
{
    private readonly ILogger<ProviderConnectionController> _logger;
    private readonly IMapper _mapper;
    private readonly IProviderConnectionService _providerConnectionService;

    public ProviderConnectionController(ILogger<ProviderConnectionController> logger, 
                                        IMapper mapper, 
                                        IProviderConnectionService providerConnectionService)
    {
        _logger = logger;
        _mapper = mapper;
        _providerConnectionService = providerConnectionService;
    }


    [HttpGet]
    public async Task<IActionResult> GetConnectionsByProviderAsync(string providerId)
    {
        var providerConnections = await _providerConnectionService.GetConnectionsByProviderIdAsync(providerId);
        var providerConnectionsDtos = _mapper.Map<IEnumerable<ProviderConnectionDto>>(providerConnections);

        return Ok(providerConnectionsDtos);
    }

    [HttpGet("{providerConnectionId}", Name = "GetProviderConnectionById")]
    public async Task<IActionResult> GetProviderConnectionByIdAsync(string providerId, string providerConnectionId)
    {
        var providerConnection = await _providerConnectionService.GetProviderConnectionByIdAsync(providerId, providerConnectionId);
        var providerConnectionDto = _mapper.Map<ProviderConnectionDto>(providerConnection);

        return Ok(providerConnectionDto);
    }

    [HttpPost]
    public async Task<IActionResult> AddProviderConnectionToProviderAsync(string providerId, [FromBody] CreateProviderConnectionRequest createProviderConnectionRequest)
    {
        var providerConnection = await _providerConnectionService.AddProviderConnectionToProviderAsync(providerId, createProviderConnectionRequest);
        var providerConnectionDto = _mapper.Map<ProviderConnectionDto>(providerConnection);

        return CreatedAtRoute("GetProviderConnectionById", 
                              new { providerId = providerId, providerConnectionId = providerConnectionDto.Id }, 
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
