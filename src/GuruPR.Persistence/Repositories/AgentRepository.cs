using GuruPR.Application.Interfaces.Persistence;
using GuruPR.Domain.Entities;
using GuruPR.Persistence.Contexts;

namespace GuruPR.Persistence.Repositories;

public class AgentRepository : GenericRepository<Agent>, IAgentRepository
{
    public AgentRepository(GuruDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<IEnumerable<Agent>> GetAllAgentsAsync(string userId)
    {
        throw new NotImplementedException();
    }
}
