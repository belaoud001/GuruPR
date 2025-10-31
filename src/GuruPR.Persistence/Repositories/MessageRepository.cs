using GuruPR.Application.Interfaces.Persistence;
using GuruPR.Domain.Entities;
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
        var baseQuery = _dbSet.Where(m => m.ConversationId == conversationId);

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
}
