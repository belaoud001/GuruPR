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
        Console.WriteLine("Getting token for authenticated calls...");
        return await Task.FromResult("123456789");
    }


    [KernelFunction("GetUserLikedTracks")]
    [Description("Retrieves the current user's liked tracks from Spotify")]
    public async Task<string> GetUserLikedTracksAsync(string token)
    {
        Console.WriteLine("Retrieving user's liked tracks from Spotify...");
        Console.WriteLine($"Using token: {token}");
        return await _spotifyService.GetLikedSongsAsync();
    }
}
