using GuruPR.Application.Features.ProviderConnections.Dtos;

using MediatR;

namespace GuruPR.Application.Features.ProviderConnections.Commands.UpdateProviderConnection;

public record UpdateProviderConnectionCommand : IRequest<ProviderConnectionDto>
{
    public string? ProviderId { get; init; }

    public string? ProviderConnectionId { get; init; }

    public required string ClientId { get; init; }

    public required string ClientSecret { get; init; }

    public required List<string> Scopes { get; init; }
}
