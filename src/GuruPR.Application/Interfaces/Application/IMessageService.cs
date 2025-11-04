using GuruPR.Domain.Entities.Message;

namespace GuruPR.Application.Interfaces.Application;

public interface IMessageService
{
    Task<List<Message>> GetMessagesByConversationIdAsync(string conversationId, string userId);
}
