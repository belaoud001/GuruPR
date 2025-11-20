using Asp.Versioning;

using GuruPR.Application.Features.Providers.Commands.CreateProvider;
using GuruPR.Application.Features.Providers.Commands.DeleteProvider;
using GuruPR.Application.Features.Providers.Commands.UpdateProvider;
using GuruPR.Application.Features.Providers.Dtos;
using GuruPR.Application.Features.Providers.Queries.GetProviderById;
using GuruPR.Application.Features.Providers.Queries.GetProviders;

using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GuruPR.Controllers.v1;

[Authorize(Roles = "Admin")]
[ApiController]
[ApiVersion(1.0)]
[Route("api/v{version:apiVersion}/providers")]
public class ProviderController : ControllerBase
{
    private readonly ILogger<ProviderController> _logger;
    private readonly IMediator _mediator;

    public ProviderController(ILogger<ProviderController> logger, IMediator mediator)
    {
        _logger = logger;
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<List<ProviderDto>>> GetAllProvidersAsync()
    {
        var providers = await _mediator.Send(new GetProvidersQuery());

        return Ok(providers);
    }

    [HttpGet("{providerId}", Name = "GetProviderById")]
    public async Task<ActionResult<ProviderDto>> GetProviderByIdAsync(string providerId)
    {
        var provider = await _mediator.Send(new GetProviderByIdQuery(providerId));

        return provider == null ? NotFound() : Ok(provider);
    }

    [HttpPost]
    public async Task<ActionResult<ProviderDto>> CreateProviderAsync([FromBody] CreateProviderCommand createProviderCommand)
    {
        var provider = await _mediator.Send(createProviderCommand);

        return CreatedAtRoute("GetProviderById", new { providerId = provider.Id }, provider);
    }

    [HttpPut("{providerId}")]
    public async Task<ActionResult<ProviderDto>> UpdateProviderAsync(string providerId, [FromBody] UpdateProviderCommand updateProviderCommand)
    {
        updateProviderCommand.Id = providerId;

        var provider = await _mediator.Send(updateProviderCommand);

        return Ok(provider);
    }

    [HttpDelete("{providerId}")]
    public async Task<IActionResult> DeleteProviderAsync(string providerId)
    {
        await _mediator.Send(new DeleteProviderCommand(providerId));

        return NoContent();
    }
}
