using Asp.Versioning;

using GuruPR.Application.Common.Models;
using GuruPR.Application.Features.Conversations.Commands.ClearConversation;
using GuruPR.Application.Features.Conversations.Commands.CreateCompletion;
using GuruPR.Application.Features.Conversations.Commands.CreateConversation;
using GuruPR.Application.Features.Conversations.Commands.DeleteConversation;
using GuruPR.Application.Features.Conversations.Commands.UpdateConversation;
using GuruPR.Application.Features.Conversations.Dtos;
using GuruPR.Application.Features.Conversations.Queries.GetConversationById;
using GuruPR.Application.Features.Conversations.Queries.GetConversationsByUserId;
using GuruPR.Application.Features.Conversations.Queries.GetMessages;

using MediatR;

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
    private readonly IMediator _mediator;

    public ConversationController(ILogger<ConversationController> logger, IMediator mediator)
    {
        _logger = logger;
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<PaginatedList<ConversationDto>>> GetAllConversationsByUserIdAsync([FromQuery] GetConversationsByUserIdQuery getConversationsByUserIdQuery)
    {
        var conversations = await _mediator.Send(getConversationsByUserIdQuery);

        return Ok(conversations);
    }

    [HttpGet("{conversationId}", Name = "GetConversationById")]
    public async Task<ActionResult<ConversationDto>> GetConversationByIdAsync(string conversationId)
    {
        var conversation = await _mediator.Send(new GetConversationByIdQuery(conversationId));

        return Ok(conversation);
    }

    [HttpPost]
    public async Task<ActionResult<ConversationDto>> CreateConversationAsync(CreateConversationCommand createConversationCommand)
    {
        var conversation = await _mediator.Send(createConversationCommand);

        return CreatedAtRoute("GetConversationById", new { conversationId = conversation.Id }, conversation);
    }

    [HttpPut("{conversationId}")]
    public async Task<ActionResult<ConversationDto>> UpdateConversationAsync(string conversationId, [FromBody] UpdateConversationCommand updateConversationCommand)
    {
        updateConversationCommand.Id = conversationId;

        var conversation = await _mediator.Send(updateConversationCommand);

        return Ok(conversation);
    }

    [HttpDelete("{conversationId}")]
    public async Task<IActionResult> DeleteConversationAsync(string conversationId)
    {
        await _mediator.Send(new DeleteConversationCommand(conversationId));

        return NoContent();
    }

    [HttpGet("{conversationId}/messages")]
    public async Task<ActionResult<PaginatedList<MessageDto>>> GetMessagesByConversationIdAsync(string conversationId)
    {
        var messages = await _mediator.Send(new GetMessagesQuery(conversationId));

        return Ok(messages);
    }

    [HttpGet("{conversationId}/messages/clear")]
    public async Task<IActionResult> ClearConversationAsync(string conversationId)
    {
        await _mediator.Send(new ClearConversationCommand(conversationId));

        return NoContent();
    }

    [HttpPost("{conversationId}/completions")]
    public async Task<ActionResult<MessageDto>> CreateCompletionAsync(string conversationId, [FromBody] CreateCompletionCommand createCompletionCommand)
    {
        createCompletionCommand.Id = conversationId;

        var message = await _mediator.Send(createCompletionCommand);

        // ToDo: Change to Accepted when streaming is implemented
        return NoContent();
    }
}
