using GuruPR.Application.Common.Models;
using GuruPR.Application.Features.Conversations.Dtos;

using MediatR;

namespace GuruPR.Application.Features.Conversations.Queries.GetConversationsByUserId;

public record GetConversationsByUserIdQuery : IRequest<PaginatedList<ConversationDto>>
{
    public string UserId { get; set; } = null!;

    public int Page { get; set; }

    public int PageSize { get; set; }
}
