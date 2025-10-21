using System.Net.Http.Headers;

using Microsoft.Extensions.Logging;

namespace GuruPR.Infrastructure.HttpClients.Spotify;

public class SpotifyClient
{
    private readonly HttpClient _httpClient;

    public SpotifyClient(ILogger<SpotifyClient> logger, HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<string> GetUserTracksAsync(string token, int limit)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"v1/me/tracks?limit={limit}")
                      {
                          Headers =
                          {
                              Authorization = new AuthenticationHeaderValue("Bearer", token)
                          }
                      };

        var response = await _httpClient.SendAsync(request);

        return await response.Content.ReadAsStringAsync();
    }
}
