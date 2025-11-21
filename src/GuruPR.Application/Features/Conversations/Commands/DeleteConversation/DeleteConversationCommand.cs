using GuruPR.Application.Common.Markers.Interfaces;
using GuruPR.Domain.Entities.Conversation;

using MediatR;

namespace GuruPR.Application.Features.Conversations.Commands.DeleteConversation;

public record DeleteConversationCommand(string Id) : IRequest<Unit>, IOwnedEntityRequest<Conversation>
{
    public string UserId { get; set; } = null!;
}
