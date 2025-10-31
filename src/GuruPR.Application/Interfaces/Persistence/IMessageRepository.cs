using GuruPR.Domain.Entities;

namespace GuruPR.Application.Interfaces.Persistence;

public interface IMessageRepository : IGenericRepository<Message>
{
    Task<List<Message>> GetMessagesAsync(string conversationId, int? lastMessages = null);
}
