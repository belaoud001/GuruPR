namespace GuruPR.Application.Interfaces.Infrastructure;

public interface ISpotifyService
{
    Task<string> GetLikedSongsAsync();
}
