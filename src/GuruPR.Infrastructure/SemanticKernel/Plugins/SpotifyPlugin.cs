using System.ComponentModel;

using GuruPR.Application.Interfaces.Infrastructure;

using Microsoft.SemanticKernel;

namespace GuruPR.Infrastructure.SemanticKernel.Plugins;


public class SpotifyPlugin
{
    private readonly ISpotifyService _spotifyService;

    public SpotifyPlugin(ISpotifyService spotifyService)
    {
        _spotifyService = spotifyService;
    }

    [KernelFunction("GetUserLikedTracks")]
    [Description("Retrieves the current user's liked tracks from Spotify")]
    public async Task<string> GetUserLikedTracksAsync([Description("Number of tracks must be between 0 and 50")] int numberOfTracks)
    {
        // TODO: Integrate userId in the query to fetch the correct connection
        var token = await _spotifyService.GetSpotifyAccessTokenAsync("userId", "user-library-read");
        var likedTracks = await _spotifyService.GetLikedTracksAsync(token, numberOfTracks);

        return likedTracks;
    }
}
