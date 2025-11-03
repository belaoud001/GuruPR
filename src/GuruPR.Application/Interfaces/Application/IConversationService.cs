using GuruPR.Domain.Entities;
using GuruPR.Domain.Requests;

namespace GuruPR.Application.Interfaces.Application;

public interface IConversationService
{
    Task<IEnumerable<Conversation>> GetAllConversationsByUserIdAsync(string userId);

    Task<Conversation> GetConversationByIdAsync(string conversationId, string userId);

    Task RunAgentWorkflowAsync(AgentExecutionRequest request, string userId);
}
