using GuruPR.Domain.Entities;

namespace GuruPR.Application.Interfaces.Application;

public interface IConversationService
{
    public Task<IEnumerable<Conversation>> GetAllConversationsByUserIdAsync(string userId);

    public Task<Conversation> GetConversationByIdAsync(string conversationId, string userId);
}
