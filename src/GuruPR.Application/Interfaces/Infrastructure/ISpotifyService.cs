namespace GuruPR.Application.Interfaces.Infrastructure;

public interface ISpotifyService
{
    Task<string> GetSpotifyAccessTokenAsync(string userId, string scope);

    Task<string> GetLikedTracksAsync(string token, int numberOfTracks);
}
