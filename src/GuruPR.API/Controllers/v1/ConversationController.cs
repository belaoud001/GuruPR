using Asp.Versioning;

using GuruPR.Application.Interfaces.Application;
using GuruPR.Domain.Requests;
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
    private readonly IMessageService _messageService;

    public ConversationController(ILogger<ConversationController> logger, 
                                  IConversationService conversationService,
                                  IMessageService messageService)
    {
        _logger = logger;
        _conversationService = conversationService;
        _messageService = messageService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllConversationsAsync()
    {
        var userId = User.GetClaimValue(JwtClaimTypes.Subject);
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized("Invalid token or missing subject claim.");
        }

        var conversations = await _conversationService.GetAllConversationsByUserIdAsync(userId);
        return Ok(conversations);
    }

    [HttpGet("{conversationId}")]
    public async Task<IActionResult> GetConversationByIdAsync(string conversationId)
    {
        var userId = User.GetClaimValue(JwtClaimTypes.Subject);
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized("Invalid token or missing subject claim.");
        }

        var conversation = await _conversationService.GetConversationByIdAsync(conversationId, userId);
        return Ok(conversation);
    }

    [HttpGet("{conversationId}/messages")]
    public async Task<IActionResult> GetMessagesByConversationIdAsync(string conversationId)
    {
        var userId = User.GetClaimValue(JwtClaimTypes.Subject);
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized("Invalid token or missing subject claim.");
        }

        var messages = await _messageService.GetMessagesByConversationIdAsync(conversationId, userId);
        return Ok(messages);
    }

    [HttpPost("{conversationId}/completions")]
    public async Task<IActionResult> CreateCompletionAsync(string conversationId, [FromBody] AgentExecutionRequest agentExecutionRequest)
    {
        var userId = User.GetClaimValue(JwtClaimTypes.Subject);
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized("Invalid token or missing subject claim.");
        }

        await _conversationService.RunAgentWorkflowAsync(agentExecutionRequest, userId);

        // ToDo: Change to Accepted when streaming is implemented
        return NoContent();
    }
}
