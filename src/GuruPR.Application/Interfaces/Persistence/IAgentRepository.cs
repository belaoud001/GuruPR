using GuruPR.Domain.Entities;

namespace GuruPR.Application.Interfaces.Persistence;

public interface IAgentRepository : IGenericRepository<Agent>
{
    Task<IEnumerable<Agent>> GetAllAgentsAsync(string userId);
}
