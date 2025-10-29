using GuruPR.Domain.Entities;

namespace GuruPR.Application.Interfaces.Application;

public interface IMessageService
{
    Task<List<Message>> GetMessagesByConversationIdAsync(string conversationId, string userId);
}
