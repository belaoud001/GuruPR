using GuruPR.Application.Features.ProviderConnections.Dtos;

using MediatR;

namespace GuruPR.Application.Features.ProviderConnections.Commands.CreateProviderConnection;

public record CreateProviderConnectionCommand : IRequest<ProviderConnectionDto>
{
    public string? ProviderId { get; set; }

    public required string ClientId { get; init; }

    public required string ClientSecret { get; init; }

    public required string AccessToken { get; init; }

    public required string RefreshToken { get; init; }

    public required List<string> Scopes { get; init; }

    public required DateTime AccessExpiresAt { get; init; }
}
