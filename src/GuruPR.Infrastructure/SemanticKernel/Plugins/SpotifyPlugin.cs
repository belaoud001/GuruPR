using System.ComponentModel;
using Microsoft.SemanticKernel;

using GuruPR.Application.Interfaces.Infrastructure;

namespace GuruPR.Infrastructure.SemanticKernel.Plugins;

public class SpotifyPlugin
{
    private readonly ISpotifyService _spotifyService;

    public SpotifyPlugin(ISpotifyService spotifyService)
    {
        _spotifyService = spotifyService;
    }

    [KernelFunction("GetToken")]
    [Description("Get token to perform authenticated calls")]
    public async Task<string> GetTokenForAuthCallsAsync()
    {
        return await Task.FromResult("123456789");
    }

    [KernelFunction("GetUserLikedTracks")]
    [Description("Retrieves the current user's liked tracks from Spotify")]
    public async Task<string> GetUserLikedTracksAsync([Description("Number of tracks must be between 0 and 50")] int numberOfTracks)
    {
        var token = await _spotifyService.GetSpotifyAccessTokenAsync("userId", "user-library-read");
        var likedTracks = await _spotifyService.GetLikedTracksAsync(token, numberOfTracks);

        return likedTracks;
    }
}
