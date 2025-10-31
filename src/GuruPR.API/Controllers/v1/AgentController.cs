using Asp.Versioning;

using GuruPR.Application.Interfaces.Application;
using GuruPR.Extensions;
using GuruPR.Infrastructure.Identity.Constants;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GuruPR.Controllers.v1;

[Authorize]
[ApiController]
[ApiVersion(1.0)]
[Route("api/v{version:apiVersion}/agents")]
public class AgentController : ControllerBase
{
    private readonly ILogger<AgentController> _logger;
    private readonly IAgentService _agentService;

    public AgentController(ILogger<AgentController> logger,
                           IAgentService agentService)
    {
        _logger = logger;
        _agentService = agentService;
    }

    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAllAgentsAsync()
    {
        var agents = await _agentService.GetAllAgentsAsync();
        return Ok(agents);
    }

    [HttpGet("/me")]
    public async Task<IActionResult> GetAllAgentsForUserAsync(string agentId)
    {
        var userId = User.GetClaimValue(JwtClaimTypes.Subject);

        var agent = await _agentService.GetAllAgentsAsync(userId);
        return Ok(agent);
    }
}
