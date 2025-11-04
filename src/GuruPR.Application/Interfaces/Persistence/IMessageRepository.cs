using GuruPR.Domain.Entities.Message;

namespace GuruPR.Application.Interfaces.Persistence;

public interface IMessageRepository : IGenericRepository<Message>
{
    Task<List<Message>> GetMessagesAsync(string conversationId, int? lastMessages = null);

    Task DeleteConversationMessagesAsync(string conversationId);
}
