using GuruPR.Application.Interfaces.Persistence;
using GuruPR.Domain.Entities.Message;
using GuruPR.Persistence.Contexts;

using Microsoft.EntityFrameworkCore;

namespace GuruPR.Persistence.Repositories;

public class MessageRepository : GenericRepository<Message>, IMessageRepository
{
    public MessageRepository(GuruDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<List<Message>> GetMessagesAsync(string conversationId, int? lastMessages = null)
    {
        var baseQuery = _dbSet.Where(message => message.ConversationId == conversationId);

        if (lastMessages.HasValue && lastMessages.Value > 0)
        {
            var recent = await baseQuery.OrderByDescending(m => m.CreatedAt)
                                        .Take(lastMessages.Value)
                                        .ToListAsync();

            recent.Reverse();

            return recent;
        }

        return await baseQuery.OrderBy(m => m.CreatedAt)
                              .ToListAsync();
    }

    public async Task DeleteConversationMessagesAsync(string conversationId)
    {
        var messages = await _dbSet.Where(message => message.ConversationId == conversationId)
                                   .ExecuteDeleteAsync();
    }
}
