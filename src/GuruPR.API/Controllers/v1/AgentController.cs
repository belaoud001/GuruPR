using Asp.Versioning;

using GuruPR.Application.Features.Agents.Commands.CreateAgent;
using GuruPR.Application.Features.Agents.Commands.DeleteAgent;
using GuruPR.Application.Features.Agents.Commands.UpdateAgent;
using GuruPR.Application.Features.Agents.Queries.GetAgentById;
using GuruPR.Application.Features.Agents.Queries.GetAgents;

using MediatR;

using Microsoft.AspNetCore.Mvc;

namespace GuruPR.Controllers.v1;

//[Authorize]
[ApiController]
[ApiVersion(1.0)]
[Route("api/v{version:apiVersion}/agents")]
public class AgentController : ControllerBase
{
    private readonly ILogger<AgentController> _logger;
    private readonly IMediator _mediator;

    public AgentController(ILogger<AgentController> logger,
                           IMediator mediator)
    {
        _logger = logger;
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllAgentsAsync([FromBody] GetAgentsQuery getAgentsQuery)
    {
        var agents = await _mediator.Send(getAgentsQuery);

        return Ok(agents);
    }

    [HttpGet("{agentId}", Name = "GetAgentById")]
    public async Task<IActionResult> GetAgentByIdAsync([FromRoute] string agentId)
    {
        var getAgentByIdQuery = new GetAgentByIdQuery(agentId);
        var agent = await _mediator.Send(getAgentByIdQuery);

        return Ok(agent);
    }

    [HttpPost]
    public async Task<IActionResult> CreateAgentAsync([FromBody] CreateAgentCommand createAgentCommand)
    {
        var agent = await _mediator.Send(createAgentCommand);

        return CreatedAtRoute("GetAgentById", new { agentId = agent.Id }, agent);
    }

    [HttpPut("{agentId}")]
    public async Task<IActionResult> UpdateAgentAsync([FromRoute] string agentId, [FromBody] UpdateAgentCommand updateAgentCommand)
    {
        updateAgentCommand.Id = agentId;

        var agent = await _mediator.Send(updateAgentCommand);

        return Ok(agent);
    }

    [HttpDelete("{agentId}")]
    public async Task<IActionResult> DeleteAgentAsync([FromRoute] string agentId)
    {
        var deleteAgentCommand = new DeleteAgentCommand(agentId);

        await _mediator.Send(deleteAgentCommand);

        return NoContent();
    }
}
