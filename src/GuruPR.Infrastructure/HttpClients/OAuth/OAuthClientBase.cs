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

    public virtual async Task<TokenResponse> RefreshTokenAsync(string refreshToken)
    {
        var requestBody = GetRefreshTokenParams(refreshToken);
        var response = await _httpClient.PostAsync(TokenEndpoint, new FormUrlEncodedContent(requestBody));

        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        return ParseTokenResponse(content);
    }

    protected abstract string TokenEndpoint { get; }

    protected abstract TokenResponse ParseTokenResponse(string json);
}
