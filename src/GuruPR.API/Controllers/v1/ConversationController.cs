using Asp.Versioning;

using AutoMapper;

using GuruPR.Application.Dtos.Conversation;
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
    private readonly IMapper _mapper;
    private readonly IConversationService _conversationService;
    private readonly IMessageService _messageService;

    public ConversationController(ILogger<ConversationController> logger,
                                  IMapper mapper,
                                  IConversationService conversationService,
                                  IMessageService messageService)
    {
        _logger = logger;
        _mapper = mapper;
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

    [HttpGet("{conversationId}", Name = "GetConversationById")]
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

    [HttpPost]
    public async Task<IActionResult> CreateConversationAsync(CreateConversationRequest createConversationRequest)
    {
        var conversation = await _conversationService.CreateConversationAsync(createConversationRequest);
        var conversationDto = _mapper.Map<ConversationDto>(conversation);

        return CreatedAtRoute("GetConversationById", new { conversationId = conversation.Id }, conversationDto);
    }

    [HttpPut("{conversationId}")]
    public async Task<IActionResult> UpdateConversationAsync(string conversationId, [FromBody] UpdateConversationRequest updateConversationRequest)
    {
        var userId = User.GetClaimValue(JwtClaimTypes.Subject);
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized("Invalid token or missing subject claim.");
        }

        var conversation = await _conversationService.UpdateConversationAsync(conversationId, userId, updateConversationRequest);
        var conversationDto = _mapper.Map<ConversationDto>(conversation);

        return Ok(conversationDto);
    }

    [HttpDelete("{conversationId}")]
    public async Task<IActionResult> DeleteConversationAsync([FromQuery] string conversationId)
    {
        var result = await _conversationService.DeleteConversationAsync(conversationId);

        return result ? NoContent() : NotFound();
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
