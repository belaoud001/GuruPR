using GuruPR.Application.Features.Conversations.Dtos;

using MediatR;

namespace GuruPR.Application.Features.Conversations.Commands.CreateConversation;

public class CreateConversationCommand : IRequest<ConversationDto>
{
    public string UserId { get; set; } = null!;

    public string AgentId { get; set; } = null!;

    public string Title { get; set; } = null!;
}
