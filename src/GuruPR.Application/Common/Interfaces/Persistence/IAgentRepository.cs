using GuruPR.Domain.Entities.Agents;

namespace GuruPR.Application.Common.Interfaces.Persistence;

public interface IAgentRepository : IGenericRepository<Agent>
{
    Task<List<Agent>> GetAllAgentsAsync(string userId);
}
