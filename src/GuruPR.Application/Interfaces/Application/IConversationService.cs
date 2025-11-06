using GuruPR.Application.Dtos.Conversation;
using GuruPR.Domain.Entities.Conversation;
using GuruPR.Domain.Requests;

namespace GuruPR.Application.Interfaces.Application;

public interface IConversationService
{
    Task<List<Conversation>> GetAllConversationsByUserIdAsync(string userId);

    Task<Conversation> GetConversationByIdAsync(string conversationId, string userId);

    Task<Conversation> CreateConversationAsync(CreateConversationRequest createConversationRequest, string userId);

    Task<Conversation> UpdateConversationAsync(string conversationId, string userId, UpdateConversationRequest updateConversationRequest);

    Task<bool> DeleteConversationAsync(string conversationId);

    Task RunAgentWorkflowAsync(AgentExecutionRequest request, string userId);
}
