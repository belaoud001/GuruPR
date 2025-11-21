using System.Text.Json.Serialization;

using GuruPR.Application.Common.Markers.Interfaces;
using GuruPR.Application.Common.Models;
using GuruPR.Application.Features.Conversations.Dtos;

using MediatR;

namespace GuruPR.Application.Features.Conversations.Queries.GetConversationsByUserId;

public record GetConversationsByUserIdQuery(int Page, int PageSize) : IRequest<PaginatedList<ConversationDto>>, IUserContextCommand
{
    [JsonIgnore]
    public string? UserId { get; set; } = null!;
}
