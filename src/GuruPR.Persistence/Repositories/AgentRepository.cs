using GuruPR.Application.Common.Interfaces.Persistence;
using GuruPR.Domain.Entities.Agents;
using GuruPR.Persistence.Contexts;

using Microsoft.EntityFrameworkCore;

namespace GuruPR.Persistence.Repositories;

public class AgentRepository : OwnedGenericRepository<Agent>, IAgentRepository
{
    public AgentRepository(GuruDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<List<Agent>> GetAllAgentsAsync(string userId)
    {
        return await _dbSet.Where(agent => agent.UserId == userId)
                           .ToListAsync();
    }
}
