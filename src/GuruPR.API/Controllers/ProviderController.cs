using Microsoft.AspNetCore.Mvc;

using GuruPR.Application.Interfaces.Application;
using GuruPR.Application.Dtos.OAuth.Provider;

namespace GuruPR.Controllers;

[ApiController]
[Route("api/v1/providers")]
public class ProviderController : ControllerBase
{
    private readonly ILogger<ProviderController> _logger;
    private readonly IProviderService _providerService;


    public ProviderController(ILogger<ProviderController> logger, IProviderService providerService)
    {
        _logger = logger;
        _providerService = providerService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllProviders()
    {
        var providers = await _providerService.GetAllProvidersAsync();
        return Ok(providers);
    }

    [HttpGet("{providerId}")]
    public async Task<IActionResult> GetProviderById(string providerId)
    {
        var provider = await _providerService.GetProviderByIdAsync(providerId);

        if (provider == null)
        {
            return NotFound();
        }
        return Ok(provider);
    }

    [HttpPost]
    public async Task<IActionResult> CreateProvider([FromBody] CreateProviderRequest createProviderRequest)
    {
        var provider = await _providerService.CreateProviderAsync(createProviderRequest);

        return CreatedAtAction(nameof(GetProviderById), new { providerId = provider.Id }, provider);
    }

    [HttpPut("{providerId}")]
    public async Task<IActionResult> UpdateProvider(string providerId, [FromBody] UpdateProviderRequest updateProviderRequest)
    {
        var provider = await _providerService.UpdateProviderAsync(providerId, updateProviderRequest);

        if (provider == null)
        {
            return NotFound();
        }
        return Ok(provider);
    }

    [HttpDelete("{providerId}")]
    public async Task<IActionResult> DeleteProvider(string providerId)
    {
        var result = await _providerService.DeleteProviderAsync(providerId);
        if (!result)
        {
            return NotFound();
        }

        return NoContent();
    }
}
