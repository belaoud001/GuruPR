using GuruPR.Application.Dtos.Agent;
using GuruPR.Domain.Entities;

namespace GuruPR.Application.Interfaces.Application;

public interface IAgentService
{
    Task<IEnumerable<Agent>> GetAllAgentsAsync(string? userId = null);

    Task<Agent> GetAgentByIdAsync(string agentId);

    Task<Agent> CreateAgentAsync(CreateAgentRequest createAgentRequest, string userId);

    Task<Agent> UpdateAgentAsync(string agentId, UpdateAgentRequest updateAgentRequest);

    Task<bool> DeleteAgentAsync(string agentId);
}
