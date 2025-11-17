using GuruPR.Application.Common.Interfaces.Persistence;
using GuruPR.Application.Features.Providers.Exceptions;
using GuruPR.Domain.Entities.Provider;

namespace GuruPR.Application.Features.Providers.Extensions;

public static class ProviderRespositoryExtensions
{
    public static async Task<Provider> GetByIdOrThrowAsync(this IProviderRepository providerRepository,
                                                                string id,
                                                                CancellationToken cancellationToken = default)
    {
        var provider = await providerRepository.GetByIdAsync(id, cancellationToken);

        return provider ?? throw new ProviderNotFoundException($"Provider with the given ID {id} not found.");
    }
}
