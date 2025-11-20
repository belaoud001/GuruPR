using System.Text.Json.Serialization;

using GuruPR.Application.Common.Markers.Interfaces;
using GuruPR.Domain.Entities.Conversation;

using MediatR;

namespace GuruPR.Application.Features.Conversations.Commands.ClearConversation;

public record ClearConversationCommand(string Id) : IRequest<Unit>, IOwnedEntityRequest<Conversation>
{

    [JsonIgnore]
    public string UserId { get; set; } = string.Empty;
}
