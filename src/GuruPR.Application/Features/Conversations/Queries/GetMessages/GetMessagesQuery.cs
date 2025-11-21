using GuruPR.Application.Features.Conversations.Dtos;

using MediatR;

namespace GuruPR.Application.Features.Conversations.Queries.GetMessages;

public record GetMessagesQuery(string ConversationId) : IRequest<List<MessageDto>>;
