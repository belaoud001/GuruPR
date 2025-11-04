using GuruPR.Application.Exceptions;
using GuruPR.Application.Interfaces.Application;
using GuruPR.Application.Interfaces.Persistence;
using GuruPR.Domain.Entities.Message;

namespace GuruPR.Application.Services;

public class MessageService : IMessageService
{
    private readonly IUnitOfWork _unitOfWork;

    public MessageService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<List<Message>> GetMessagesByConversationIdAsync(string conversationId, string userId)
    {
        var conversation = await _unitOfWork.Conversations.GetByIdAsync(conversationId);

        if (conversation == null)
        {
            throw new NotFoundException($"Conversation with given ID {conversationId} not found.");
        }

        if (conversation.UserId != userId)
        {
            throw new UnauthorizedAccessException("User does not have access to this conversation.");
        }

        var messages = await _unitOfWork.Messages.GetMessagesAsync(conversationId);

        return messages;
    }
}
