using GuruPR.Application.Interfaces.Infrastructure;

namespace GuruPR.Infrastructure.Services.ThirdParties;

public class SpotifyService : ISpotifyService
{
    public Task<string> GetLikedSongsAsync()
    {
        return Task.FromResult("zbi;tbon;eurika;reverie");
    }
}
