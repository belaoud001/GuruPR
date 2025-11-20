using GuruPR.Application.Common.Markers.Interfaces;
using GuruPR.Application.Features.Conversations.Dtos;
using GuruPR.Domain.Entities.Conversation;

using MediatR;

namespace GuruPR.Application.Features.Conversations.Queries.GetConversationById;

public record GetConversationByIdQuery(string Id) : IRequest<ConversationDto>, IOwnedEntityRequest<Conversation>
{
    public string UserId { get; set; } = null!;
}
