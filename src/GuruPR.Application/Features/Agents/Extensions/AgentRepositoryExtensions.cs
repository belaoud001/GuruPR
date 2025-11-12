using GuruPR.Application.Features.Agents.Exceptions;
using GuruPR.Application.Interfaces.Persistence;
using GuruPR.Domain.Entities.Agents;

namespace GuruPR.Application.Features.Agents.Extensions;

public static class AgentRepositoryExtensions
{
    public static async Task<Agent> GetByIdOrThrowAsync(this IAgentRepository agentRepository,
                                                             string id,
                                                             CancellationToken cancellationToken = default)
    {
        var agent = await agentRepository.GetByIdAsync(id, cancellationToken);

        return agent ?? throw new AgentNotFoundException($"Agent with the given ID {id} not found.");
    }
}
