using System.Text.Json.Serialization;

using GuruPR.Application.Common.Markers.Interfaces;
using GuruPR.Application.Features.Conversations.Dtos;
using GuruPR.Domain.Entities.Conversation;

using MediatR;

namespace GuruPR.Application.Features.Conversations.Commands.CreateCompletion;

public class CreateCompletionCommand : IRequest<MessageDto>, IOwnedEntityRequest<Conversation>
{
    public string Id { get; set; } = null!;

    [JsonIgnore]
    public string UserId { get; set; } = null!;

    public string ConversationId { get; set; } = null!;

    public string Message { get; set; } = null!;

    public bool StreamResponse { get; set; }
}
