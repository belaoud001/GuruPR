using GuruPR.Application.Common.Interfaces.Persistence;
using GuruPR.Application.Features.ProviderConnections.Exceptions;
using GuruPR.Domain.Entities.OAuth;

namespace GuruPR.Application.Features.ProviderConnections.Extensions;

public static class ProviderConnectionRepositoryExtensions
{
    public static async Task<ProviderConnection> GetByIdOrThrowAsync(this IProviderConnectionRepository providerConnectionRepository,
                                                                          string id,
                                                                          CancellationToken cancellationToken = default)
    {
        var providerConnection = await providerConnectionRepository.GetByIdAsync(id, cancellationToken);

        return providerConnection ?? throw new ProviderConnectionNotFoundException($"Provider connection with the given ID {id} not found.");
    }
}
