using Asp.Versioning;

using AutoMapper;

using GuruPR.Application.Common.Interfaces.Application;
using GuruPR.Application.Dtos.OAuth.Provider;
using GuruPR.Application.Features.Providers.Dtos;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GuruPR.Controllers.v1;

[Authorize]
[ApiController]
[ApiVersion(1.0)]
[Route("api/v{version:apiVersion}/providers")]
public class ProviderController : ControllerBase
{
    private readonly ILogger<ProviderController> _logger;
    private readonly IMapper _mapper;
    private readonly IProviderService _providerService;

    public ProviderController(ILogger<ProviderController> logger, IMapper mapper, IProviderService providerService)
    {
        _logger = logger;
        _mapper = mapper;
        _providerService = providerService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllProvidersAsync()
    {
        var providers = await _providerService.GetAllProvidersAsync();
        var providerDtos = _mapper.Map<IEnumerable<ProviderDto>>(providers);

        return Ok(providerDtos);
    }

    [HttpGet("{providerId}", Name = "GetProviderById")]
    public async Task<IActionResult> GetProviderByIdAsync(string providerId)
    {
        var provider = await _providerService.GetProviderByIdAsync(providerId);
        var providerDto = _mapper.Map<ProviderDto>(provider);

        return provider == null ? NotFound() : Ok(providerDto);
    }

    [HttpPost]
    public async Task<IActionResult> CreateProviderAsync([FromBody] CreateProviderRequest createProviderRequest)
    {
        var provider = await _providerService.CreateProviderAsync(createProviderRequest);
        var providerDto = _mapper.Map<ProviderDto>(provider);

        return CreatedAtRoute("GetProviderById", new { providerId = providerDto.Id }, providerDto);
    }

    [HttpPut("{providerId}")]
    public async Task<IActionResult> UpdateProviderAsync(string providerId, [FromBody] UpdateProviderRequest updateProviderRequest)
    {
        var provider = await _providerService.UpdateProviderAsync(providerId, updateProviderRequest);
        var providerDto = _mapper.Map<ProviderDto>(provider);

        return provider == null ? NotFound() : Ok(providerDto);
    }

    [HttpDelete("{providerId}")]
    public async Task<IActionResult> DeleteProviderAsync(string providerId)
    {
        var result = await _providerService.DeleteProviderAsync(providerId);

        return result ? NotFound() : NoContent();
    }
}
