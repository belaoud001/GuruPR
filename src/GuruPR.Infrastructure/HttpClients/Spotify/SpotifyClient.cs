using System.Text.Json;
using System.Net.Http.Headers;
using Microsoft.Extensions.Logging;

using GuruPR.Domain.Entities.OAuth;
using GuruPR.Infrastructure.HttpClients.OAuth;

namespace GuruPR.Infrastructure.HttpClients.Spotify;

public class SpotifyClient
{
    private readonly HttpClient _httpClient;

    public SpotifyClient(ILogger<SpotifyClient> logger, HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<TokenResponse> RefreshTokenAsync(ProviderConnection providerConnection)
    {
        if (string.IsNullOrWhiteSpace(providerConnection.RefreshToken))
        {
            throw new ArgumentException("Refresh token cannot be null or empty.", nameof(providerConnection.RefreshToken));
        }

        var requestBody = new Dictionary<string, string>
                          {
                              { "grant_type", "refresh_token" },
                              { "refresh_token", providerConnection.RefreshToken }
                          };

        using var request = new HttpRequestMessage(HttpMethod.Post, "https://accounts.spotify.com/api/token")
                            {
                                Content = new FormUrlEncodedContent(requestBody)
                            };

        var response = await _httpClient.SendAsync(request);
        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            throw new InvalidOperationException($"Failed to refresh Spotify token.");
        }

        var content = await response.Content.ReadAsStringAsync();
        var jsonSerializerOptions = new JsonSerializerOptions
                                    {
                                        PropertyNameCaseInsensitive = true
                                    };
        var tokenResponse = JsonSerializer.Deserialize<TokenResponse>(content, jsonSerializerOptions);

        if (tokenResponse == null || string.IsNullOrEmpty(tokenResponse.AccessToken))
        {
            throw new InvalidOperationException("Invalid token response from Spotify.");
        }

        return tokenResponse;
    }

    public async Task<string> GetUserTracksAsync(string token, int limit)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"v1/me/tracks?limit={limit}");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await _httpClient.SendAsync(request);

        return await response.Content.ReadAsStringAsync();
    }
}
