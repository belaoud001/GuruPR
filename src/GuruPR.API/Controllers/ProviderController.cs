using Microsoft.AspNetCore.Mvc;

using GuruPR.Application.Dtos.OAuth.Provider;
using GuruPR.Application.Interfaces.Application;

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
    public async Task<IActionResult> GetAllProvidersAsync()
    {
        var providers = await _providerService.GetAllProvidersAsync();
        return Ok(providers);
    }

    [HttpGet("{providerId}", Name = "GetProviderById")]
    public async Task<IActionResult> GetProviderByIdAsync(string providerId)
    {
        var provider = await _providerService.GetProviderByIdAsync(providerId);

        return provider == null ? NotFound() : Ok(provider);
    }

    [HttpPost]
    public async Task<IActionResult> CreateProviderAsync([FromBody] CreateProviderRequest createProviderRequest)
    {
        var provider = await _providerService.CreateProviderAsync(createProviderRequest);

        return CreatedAtRoute("GetProviderById", new { providerId = provider.Id }, provider);
    }

    [HttpPut("{providerId}")]
    public async Task<IActionResult> UpdateProviderAsync(string providerId, [FromBody] UpdateProviderRequest updateProviderRequest)
    {
        var provider = await _providerService.UpdateProviderAsync(providerId, updateProviderRequest);

        return provider == null ? NotFound() : Ok(provider);
    }

    [HttpDelete("{providerId}")]
    public async Task<IActionResult> DeleteProviderAsync(string providerId)
    {
        var result = await _providerService.DeleteProviderAsync(providerId);

        return result ? NotFound() : NoContent();
    }
}
