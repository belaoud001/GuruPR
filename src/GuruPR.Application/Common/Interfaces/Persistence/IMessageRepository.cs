using GuruPR.Domain.Entities.Message;

namespace GuruPR.Application.Common.Interfaces.Persistence;

public interface IMessageRepository : IGenericRepository<Message>
{
    Task<List<Message>> GetMessagesAsync(string conversationId, int? lastMessages = null, CancellationToken cancellationToken = default);

    Task DeleteConversationMessagesAsync(string conversationId, CancellationToken cancellationToken = default);
}
