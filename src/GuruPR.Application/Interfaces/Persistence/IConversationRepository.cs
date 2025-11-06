using GuruPR.Domain.Entities.Conversation;

namespace GuruPR.Application.Interfaces.Persistence;

public interface IConversationRepository : IGenericRepository<Conversation>
{
    Task<List<Conversation>> GetAllByUserIdAsync(string userId);
}
