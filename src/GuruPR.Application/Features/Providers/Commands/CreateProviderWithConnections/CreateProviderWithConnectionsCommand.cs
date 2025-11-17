using GuruPR.Application.Features.ProviderConnections.Commands.CreateProviderConnection;
using GuruPR.Application.Features.Providers.Dtos;
using GuruPR.Domain.Entities.Provider.Enums;

using MediatR;

namespace GuruPR.Application.Features.Providers.Commands.CreateProviderWithConnections;

public record CreateProviderWithConnectionsCommand : IRequest<ProviderDto>
{
    public required string ProviderId { get; init; }

    public required string DisplayName { get; init; }

    public required OAuthProviderType ProviderType { get; init; }

    public required string AuthorizationUrl { get; init; }

    public required string TokenUrl { get; init; }

    public List<string>? DefaultScopes { get; init; } = [];
}
