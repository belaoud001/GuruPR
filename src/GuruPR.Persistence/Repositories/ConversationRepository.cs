using GuruPR.Application.Interfaces.Persistence;
using GuruPR.Domain.Entities.Conversation;
using GuruPR.Persistence.Contexts;

using Microsoft.EntityFrameworkCore;

namespace GuruPR.Persistence.Repositories;

public class ConversationRepository : GenericRepository<Conversation>, IConversationRepository
{
    public ConversationRepository(GuruDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<IEnumerable<Conversation>> GetAllByUserIdAsync(string userId)
    {
        var conversations = await _dbSet.Where(conversation => conversation.UserId == userId)
                                        .ToListAsync();

        return conversations;
    }
}
