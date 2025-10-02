using System.Text;
using System.Net.Http.Headers;

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

    public virtual async Task<TokenResponse> RefreshTokenAsync(string clientId, string clientSecret, string refreshToken)
    {
        if (string.IsNullOrWhiteSpace(refreshToken))
        {
            throw new ArgumentException("Refresh token cannot be null or empty.", nameof(refreshToken));
        }

        var credentials = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{clientId}:{clientSecret}"));
        var requestBody = GetRefreshTokenParams(refreshToken);
        using var request = new HttpRequestMessage(HttpMethod.Post, TokenEndpoint)
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
