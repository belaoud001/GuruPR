using GuruPR.Domain.Entities;

namespace GuruPR.Application.Interfaces.Persistence;

public interface IUserRepository
{
    Task<User?> GetUserByRefreshTokenAsync(string refreshToken);
}
