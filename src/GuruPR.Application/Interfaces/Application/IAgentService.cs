using GuruPR.Domain.Entities;
using GuruPR.Domain.Requests;

namespace GuruPR.Application.Interfaces.Application;

public interface IAgentService
{
    Task<IEnumerable<Agent>> GetAllAgentsAsync(string? userId = null);

    Task<string> ExecuteAgentAsync(AgentExecutionRequest request, string userId);
}
