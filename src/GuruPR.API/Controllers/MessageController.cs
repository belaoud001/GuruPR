using GuruPR.Application.Interfaces.Application;
using GuruPR.Extensions;
using GuruPR.Infrastructure.Identity.Constants;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GuruPR.Controllers;

[Authorize]
[ApiController]
public class MessageController : ControllerBase
{
    private readonly IMessageService _messageService;

    public MessageController(IMessageService messageService)
    {
        _messageService = messageService;
    }

    [HttpGet("api/v1/conversations/{conversationId}/messages")]
    public async Task<IActionResult> GetMessagesByConversationId(string conversationId)
    {
        var userId = User.GetClaimValue(JwtClaimTypes.Subject);

        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized("No Subject claim was found");
        }

        var messages = await _messageService.GetMessagesByConversationIdAsync(conversationId, userId);
        return Ok(messages);
    }
}
