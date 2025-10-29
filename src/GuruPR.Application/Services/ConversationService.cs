using GuruPR.Application.Exceptions;
using GuruPR.Application.Interfaces.Application;
using GuruPR.Application.Interfaces.Persistence;
using GuruPR.Domain.Entities;

namespace GuruPR.Application.Services;

public class ConversationService : IConversationService
{
    private readonly IUnitOfWork _unitOfWork;

    public ConversationService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<Conversation>> GetAllConversationsByUserIdAsync(string userId)
    {
        var conversations = await _unitOfWork.Conversations.GetAllByUserIdAsync(userId);

        return conversations;
    }

    public async Task<Conversation> GetConversationByIdAsync(string conversationId, string userId)
    {
        var conversation = await _unitOfWork.Conversations.GetByIdAsync(conversationId);

        if (conversation == null)
        {
            throw new NotFoundException("Conversation with the given ID {conversationId} not found");
        }

        if (conversation.UserId != userId)
        {
            throw new UnauthorizedAccessException("User does not have access to this conversation.");
        }

        return conversation;
    }
}
