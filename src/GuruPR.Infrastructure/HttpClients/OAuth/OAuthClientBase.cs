using System.Net.Http.Headers;
using System.Text;

using GuruPR.Domain.Entities.OAuth;

namespace GuruPR.Infrastructure.HttpClients.OAuth;

public abstract class OAuthClientBase
{
    protected readonly HttpClient _httpClient;

    protected OAuthClientBase(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    protected virtual Dictionary<string, string> GetRefreshTokenParams(string refreshToken)
    {
        return new Dictionary<string, string>
        {
            { "grant_type", "refresh_token" },
            { "refresh_token", refreshToken }
        };
    }

    public virtual async Task<TokenResponse> RefreshTokenAsync(string tokenUrl, ProviderConnection providerConnection)
    {
        if (string.IsNullOrWhiteSpace(providerConnection.RefreshToken))
        {
            throw new ArgumentException("Refresh token cannot be null or empty.", nameof(providerConnection.RefreshToken));
        }

        var credentials = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{providerConnection.ClientId}:{providerConnection.ClientSecret}"));
        var requestBody = GetRefreshTokenParams(providerConnection.RefreshToken);
        using var request = new HttpRequestMessage(HttpMethod.Post, tokenUrl)
        {
            Content = new FormUrlEncodedContent(requestBody),
            Headers =
            {
                Authorization = new AuthenticationHeaderValue("basic", credentials)
            }
        };
        var response = await _httpClient.SendAsync(request);

        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        return ParseTokenResponse(content);
    }

    protected abstract string TokenEndpoint { get; }

    protected abstract TokenResponse ParseTokenResponse(string json);
}
