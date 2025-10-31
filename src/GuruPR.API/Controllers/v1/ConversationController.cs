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
[Route("api/v{version:apiVersion}/conversations")]
public class ConversationController : ControllerBase
{
    private readonly ILogger<ConversationController> _logger;
    private readonly IConversationService _conversationService;

    public ConversationController(ILogger<ConversationController> logger, IConversationService conversationService)
    {
        _logger = logger;
        _conversationService = conversationService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllConversations()
    {
        var userId = User.GetClaimValue(JwtClaimTypes.Subject);
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized("No Subject claim was found");
        }

        var conversations = await _conversationService.GetAllConversationsByUserIdAsync(userId);
        return Ok(conversations);
    }

    [HttpGet("{conversationId}")]
    public async Task<IActionResult> GetConversationById(string conversationId)
    {
        var userId = User.GetClaimValue(JwtClaimTypes.Subject);
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized("No Subject claim was found");
        }

        var conversation = await _conversationService.GetConversationByIdAsync(conversationId, userId);
        return Ok(conversation);
    }
}
