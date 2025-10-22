using System.Text;
using System.Text.Json;

using GuruPR.Application.Dtos.OAuth.ProviderConnection;
using GuruPR.Application.Interfaces.Application;
using GuruPR.Application.Interfaces.Infrastructure;
using GuruPR.Domain.Entities.Enums;
using GuruPR.Infrastructure.HttpClients.Spotify;

using Microsoft.Extensions.Logging;

namespace GuruPR.Infrastructure.Services.ThirdParties;

public class SpotifyService : ISpotifyService
{
    private readonly ILogger<SpotifyService> _logger;
    private readonly SpotifyClient _spotifyClient;
    private readonly SpotifyOAuthClient _spotifyOAuthClient;
    private readonly IProviderService _providerService;
    private readonly IProviderConnectionService _providerConnectionService;

    public SpotifyService(ILogger<SpotifyService> logger,
                          SpotifyClient spotifyClient,
                          SpotifyOAuthClient spotifyOAuthClient,
                          IProviderService providerService,
                          IProviderConnectionService providerConnectionService)
    {
        _logger = logger;
        _spotifyClient = spotifyClient;
        _spotifyOAuthClient = spotifyOAuthClient;
        _providerService = providerService;
        _providerConnectionService = providerConnectionService;
    }

    #region Public Methods

    public async Task<string> GetSpotifyAccessTokenAsync(string userId, string scope)
    {
        //TODO: Integrate userId in the query to fetch the correct connection
        var provider = await _providerService.GetProviderByTypeAsync(OAuthProviderType.Spotify);
        var providerConnection = await _providerConnectionService.GetProviderConnectionByProviderTypeAndScopeAsync(OAuthProviderType.Spotify, scope);

        if (providerConnection == null)
        {
            throw new InvalidOperationException("No Spotify connection found for the user.");
        }

        if (providerConnection.Scopes == null || !providerConnection.Scopes.Contains(scope))
        {
            throw new InvalidOperationException($"The existing connection does not have the required scope: {scope}");
        }

        if (providerConnection.ShouldRefreshToken())
        {
            var tokenResponse = await _spotifyOAuthClient.RefreshTokenAsync(provider.TokenUrl, providerConnection);
            var updatedConnection = new UpdateProviderConnectionRequest
            {
                AccessToken = tokenResponse.AccessToken,
                RefreshToken = tokenResponse.RefreshToken ?? providerConnection.RefreshToken,
                AccessExpiresAt = DateTime.UtcNow.AddSeconds(tokenResponse.ExpiresIn)
            };

            providerConnection = await _providerConnectionService.UpdateProviderConnectionByProviderTypeAsync(OAuthProviderType.Spotify,
                                                                                                              providerConnection.Id,
                                                                                                              updatedConnection);
        }

        return providerConnection.AccessToken;
    }

    public async Task<string> GetLikedTracksAsync(string token, int numberOfTracks)
    {
        if (string.IsNullOrWhiteSpace(token))
        {
            throw new ArgumentException("Token cannot be null or empty.", nameof(token));
        }

        if (numberOfTracks is <= 0 or > 50)
        {
            throw new ArgumentOutOfRangeException(nameof(numberOfTracks));
        }

        string json = await _spotifyClient.GetUserTracksAsync(token, numberOfTracks);
        return ParseTracksFromJson(json);
    }

    #endregion

    #region Private Methods

    private string ParseTracksFromJson(string json)
    {
        var resultBuilder = new StringBuilder();

        try
        {
            using var apiDoc = JsonDocument.Parse(json);
            var items = apiDoc.RootElement.GetProperty("items");

            if (items.GetArrayLength() == 0)
            {
                return "No liked tracks found.";
            }

            foreach (var item in items.EnumerateArray())
            {
                if (!item.TryGetProperty("track", out var track))
                {
                    continue;
                }

                string name = track.TryGetProperty("name", out var nameElement)
                              ? nameElement.GetString() ?? "Unknown"
                              : "Unknown";

                string artist = "Unknown";
                if (track.TryGetProperty("artists", out var artistArray) && artistArray.GetArrayLength() > 0)
                {
                    artist = artistArray[0].TryGetProperty("name", out var artistName)
                             ? artistName.GetString() ?? "Unknown"
                             : "Unknown";
                }

                resultBuilder.AppendLine($"{name} by {artist}");
            }
        }
        catch (JsonException jsonException)
        {
            _logger.LogError(jsonException, "Error parsing Spotify API response: {Message}", jsonException.Message);
            return $"Error parsing Spotify API response.";
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Unexpected error processing Spotify API response: {Message}", exception.Message);
            return "An unexpected error occurred while processing the response.";
        }

        return resultBuilder.ToString();
    }

    #endregion
}
