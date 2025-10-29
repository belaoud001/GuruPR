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

    public async Task<List<Message>> GetMessagesbyConversationIdAsync(string conversationId)
    {
        var messages = await _dbSet.Where(message => message.ConversationId == conversationId)
                                   .OrderBy(message => message.CreatedAt)
                                   .ToListAsync();

        return messages;
    }
}
