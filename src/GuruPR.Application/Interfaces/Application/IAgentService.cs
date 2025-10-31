using GuruPR.Domain.Requests;

namespace GuruPR.Application.Interfaces.Application;

public interface IAgentService
{
    Task<string> ExecuteAgentAsync(AgentExecutionRequest request, string userId);
}
