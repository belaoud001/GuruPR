using GuruPR.Application.Features.Providers.Dtos;
using GuruPR.Domain.Entities.Provider.Enums;

using MediatR;

namespace GuruPR.Application.Features.Providers.Commands.UpdateProvider;

public record UpdateProviderCommand : IRequest<ProviderDto>
{
    public string? Id { get; set; }

    public string? DisplayName { get; init; }

    public OAuthProviderType? ProviderType { get; init; }

    public string? AuthorizationUrl { get; init; }

    public string? TokenUrl { get; init; }

    public List<string>? DefaultScopes { get; init; }
}
