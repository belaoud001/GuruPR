using GuruPR.Domain.Entities.Agents;

namespace GuruPR.Application.Interfaces.Persistence;

public interface IAgentRepository : IGenericRepository<Agent>
{
    Task<List<Agent>> GetAllAgentsAsync(string userId);
}
