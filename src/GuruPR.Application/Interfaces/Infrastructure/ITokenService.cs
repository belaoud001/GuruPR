using GuruPR.Application.Dtos.Jwt;
using GuruPR.Domain.Entities;

namespace GuruPR.Application.Interfaces.Infrastructure;

public interface ITokenService
{
    JwtTokenResult GenerateToken(User user);

    string GenerateRefreshToken();

    void WriteAuthTokenAsHttpOnlyCookie(string cookieName, string token, DateTime expiration);
}
