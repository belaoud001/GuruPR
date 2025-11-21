using System.Text.Json.Serialization;

namespace GuruPR.Domain.Entities.Provider.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum OAuthProviderType
{
    Spotify,
    Discord,
    Other
}
