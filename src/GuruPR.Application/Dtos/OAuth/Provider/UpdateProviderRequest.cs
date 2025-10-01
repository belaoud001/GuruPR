using GuruPR.Domain.Entities.Enums;

namespace GuruPR.Application.Dtos.OAuth.Provider;

public class UpdateProviderRequest
{
    public string? DisplayName { get; set; }

    public OAuthProviderType? ProviderType { get; set; }

    public string? ClientId { get; set; }

    public string? ClientSecret { get; set; }

    public string? AuthorizationUrl { get; set; }

    public string? TokenUrl { get; set; }

    public List<string>? DefaultScopes { get; set; }
}
