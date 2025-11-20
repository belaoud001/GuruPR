using GuruPR.Application.Common.Markers.Interfaces;
using GuruPR.Application.Features.Conversations.Dtos;
using GuruPR.Domain.Entities.Conversation;

using MediatR;

namespace GuruPR.Application.Features.Conversations.Commands.UpdateConversation;

public record UpdateConversationCommand : IRequest<ConversationDto>, IOwnedEntityRequest<Conversation>
{
    public string Id { get; set; } = null!;

    public string UserId { get; set; } = null!;

    public string AgentId { get; set; } = null!;

    public string Title { get; set; } = null!;
}
