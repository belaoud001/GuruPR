using GuruPR.Domain.Entities;

namespace GuruPR.Application.Common.Interfaces.Persistence;

public interface IUserRepository
{
    Task<User?> GetUserByRefreshTokenHashAsync(string refreshTokenHash);
}
