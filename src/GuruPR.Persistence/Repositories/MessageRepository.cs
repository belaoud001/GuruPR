using GuruPR.Application.Common.Interfaces.Persistence;
using GuruPR.Domain.Entities.Message;
using GuruPR.Persistence.Contexts;

using Microsoft.EntityFrameworkCore;

namespace GuruPR.Persistence.Repositories;

public class MessageRepository : GenericRepository<Message>, IMessageRepository
{
    public MessageRepository(GuruDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<List<Message>> GetMessagesAsync(string conversationId, int? lastMessages = null, CancellationToken cancellationToken = default)
    {
        var baseQuery = _dbSet.Where(message => message.ConversationId == conversationId);

        if (lastMessages.HasValue && lastMessages.Value > 0)
        {
            var recent = await baseQuery.OrderByDescending(message => message.CreatedAt)
                                        .Take(lastMessages.Value)
                                        .ToListAsync(cancellationToken);

            recent.Reverse();

            return recent;
        }

        return await baseQuery.OrderBy(m => m.CreatedAt)
                              .ToListAsync(cancellationToken);
    }

    public async Task DeleteConversationMessagesAsync(string conversationId, CancellationToken cancellationToken = default)
    {
        await _dbSet.Where(message => message.ConversationId == conversationId)
                    .ForEachAsync(message => _dbSet.Remove(message), cancellationToken);
    }
}
