using GuruPR.Application.Common.Interfaces.Persistence;
using GuruPR.Application.Features.Conversations.Exceptions;
using GuruPR.Domain.Entities.Conversation;

namespace GuruPR.Application.Features.Conversations.Extensions;

public static class ConversationRepositoryExtensions
{
    public static async Task<Conversation> GetByIdOrThrowAsync(this IConversationRepository conversationRepository,
                                                                    string id,
                                                                    CancellationToken cancellationToken)
    {
        var conversation = await conversationRepository.GetByIdAsync(id, cancellationToken);

        return conversation ?? throw new ConversationNotFoundException($"Conversation with the given ID {id} not found.");
    }
}
