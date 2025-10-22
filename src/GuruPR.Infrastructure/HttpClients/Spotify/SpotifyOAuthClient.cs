using System.Text.Json;

using GuruPR.Infrastructure.HttpClients.OAuth;

namespace GuruPR.Infrastructure.HttpClients.Spotify;

public class SpotifyOAuthClient : OAuthClientBase
{
    public SpotifyOAuthClient(HttpClient httpClient) : base(httpClient)
    {
    }

    protected override string TokenEndpoint => "api/token";

    protected override TokenResponse ParseTokenResponse(string json)
    {
        try
        {
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            var tokenResponse = JsonSerializer.Deserialize<TokenResponse>(json, options);

            if (tokenResponse == null || string.IsNullOrEmpty(tokenResponse.AccessToken))
            {
                throw new InvalidOperationException("Invalid token response from Spotify.");
            }

            return tokenResponse;
        }
        catch (JsonException jsonException)
        {
            throw new InvalidOperationException("Failed to parse token response from Spotify.", jsonException);
        }
    }
}
