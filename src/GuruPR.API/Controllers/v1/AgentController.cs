using Asp.Versioning;

using AutoMapper;

using GuruPR.Application.Dtos.Agent;
using GuruPR.Application.Interfaces.Application;
using GuruPR.Extensions;
using GuruPR.Infrastructure.Identity.Constants;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GuruPR.Controllers.v1;

[ApiController]
[ApiVersion(1.0)]
[Route("api/v{version:apiVersion}/agents")]
public class AgentController : ControllerBase
{
    private readonly ILogger<AgentController> _logger;
    private readonly IMapper _mapper;
    private readonly IAgentService _agentService;

    public AgentController(ILogger<AgentController> logger,
                           IMapper mapper,
                           IAgentService agentService)
    {
        _logger = logger;
        _mapper = mapper;
        _agentService = agentService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllAgentsAsync()
    {
        var agents = await _agentService.GetAllAgentsAsync();
        var agentDtos = _mapper.Map<IEnumerable<AgentDto>>(agents);

        return Ok(agentDtos);
    }

    [HttpGet("{agentId}")]
    public async Task<IActionResult> GetAgentByIdAsync(string agentId)
    {
        var agent = await _agentService.GetAgentByIdAsync(agentId);

        return Ok(agent);
    }

    [HttpPost]
    public async Task<IActionResult> CreateAgentAsync([FromBody] CreateAgentRequest createAgentRequest)
    {
        var userId = User.GetClaimValue(JwtClaimTypes.Name);
        if (userId == null)
        {
            return Unauthorized("Invalid token or missing subject claim.");
        }

        var agent = await _agentService.CreateAgentAsync(createAgentRequest, userId);
        var agentDto = _mapper.Map<AgentDto>(agent);

        return Ok(agentDto);
    }

    [HttpPut("{agentId}")]
    public async Task<IActionResult> UpdateAgentAsync([FromBody] UpdateAgentRequest updateAgentRequest, string agentId)
    {
        var agent = await _agentService.UpdateAgentAsync(agentId, updateAgentRequest);
        var agentDto = _mapper.Map<AgentDto>(agent);

        return Ok(agentDto);
    }

    [HttpDelete("{agentId}")]
    public async Task<IActionResult> DeleteAgentAsync(string agentId)
    {
        var result = await _agentService.DeleteAgentAsync(agentId);

        return result ? NoContent() : NotFound();
    }
}
