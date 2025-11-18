using Asp.Versioning;

using GuruPR.Application.Features.ProviderConnections.Commands.CreateProviderConnection;
using GuruPR.Application.Features.ProviderConnections.Commands.DeleteProviderConnection;
using GuruPR.Application.Features.ProviderConnections.Commands.UpdateProviderConnection;
using GuruPR.Application.Features.ProviderConnections.Dtos;
using GuruPR.Application.Features.ProviderConnections.Queries.GetProviderConnectionById;
using GuruPR.Application.Features.ProviderConnections.Queries.GetProviderConnections;

using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GuruPR.Controllers.v1;

[Authorize(Roles = "Admin")]
[ApiController]
[ApiVersion(1.0)]
[Route("api/v{version:apiVersion}/providers/{providerId}/provider-connections")]
public class ProviderConnectionController : ControllerBase
{
    private readonly ILogger<ProviderConnectionController> _logger;
    private readonly IMediator _mediator;

    public ProviderConnectionController(ILogger<ProviderConnectionController> logger,
                                        IMediator mediator)
    {
        _logger = logger;
        _mediator = mediator;
    }


    [HttpGet]
    public async Task<ActionResult<List<ProviderConnectionDto>>> GetProviderConnectionsByProviderAsync(string providerId)
    {
        var providerConnections = await _mediator.Send(new GetProviderConnectionsQuery(providerId));

        return Ok(providerConnections);
    }

    [HttpGet("{providerConnectionId}", Name = "GetProviderConnectionById")]
    public async Task<ActionResult<ProviderConnectionDto>> GetProviderConnectionByIdAsync(string providerConnectionId)
    {
        var providerConnections = await _mediator.Send(new GetProviderConnectionByIdQuery(providerConnectionId));

        return Ok(providerConnections);
    }

    [HttpPost]
    public async Task<IActionResult> AddProviderConnectionToProviderAsync(string providerId, [FromBody] CreateProviderConnectionCommand createProviderConnectionCommand)
    {
        createProviderConnectionCommand.ProviderId = providerId;
        var providerConnection = await _mediator.Send(createProviderConnectionCommand);

        return CreatedAtRoute("GetProviderConnectionById",
                              new { providerId, providerConnectionId = providerConnection.Id },
                              providerConnection);
    }

    [HttpPut("{providerConnectionId}")]
    public async Task<IActionResult> UpdateProviderConnectionAsync(string providerConnectionId, [FromBody] UpdateProviderConnectionCommand updateProviderConnectionCommand)
    {
        updateProviderConnectionCommand.Id = providerConnectionId;

        var providerConnection = await _mediator.Send(updateProviderConnectionCommand);

        return Ok(providerConnection);
    }

    [HttpDelete("{providerConnectionId}")]
    public async Task<IActionResult> DeleteProviderConnectionAsync(string providerConnectionId)
    {
        await _mediator.Send(new DeleteProviderConnectionCommand(providerConnectionId));

        return NoContent();
    }
}
