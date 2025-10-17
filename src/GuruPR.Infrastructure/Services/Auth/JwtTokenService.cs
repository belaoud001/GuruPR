using System.Text;
using System.Security.Claims;
using System.Security.Cryptography;
using System.IdentityModel.Tokens.Jwt;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

using GuruPR.Domain.Entities;
using GuruPR.Application.Dtos.Jwt;
using GuruPR.Application.Settings.Security;
using GuruPR.Application.Interfaces.Infrastructure;

namespace GuruPR.Infrastructure.Services.Auth;

public class JwtTokenService : ITokenService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly JwtSettings _jwtSettings;

    public JwtTokenService(IHttpContextAccessor httpContextAccessor, IOptions<JwtSettings> jwtSettings)
    {
        _httpContextAccessor = httpContextAccessor;
        _jwtSettings = jwtSettings.Value;
    }

    public JwtTokenResult GenerateToken(User user)
    {
        var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey));
        var signingCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub,   user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Jti,   Guid.NewGuid().ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email ?? string.Empty),
            new Claim(ClaimTypes.NameIdentifier,     user.ToString()),
        };
        var expires = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpirationTimeInMinutes);
        var token = new JwtSecurityToken(
                        issuer: _jwtSettings.Issuer,
                        audience: _jwtSettings.Audience,
                        claims: claims,
                        notBefore: DateTime.UtcNow,
                        expires: expires,
                        signingCredentials: signingCredentials
                    );
        var jwtToken = new JwtSecurityTokenHandler().WriteToken(token);
        var jwtTokenResult = new JwtTokenResult
                             {
                                 Token = jwtToken,
                                 ExpiresAtUtc = expires
                             };

        return jwtTokenResult;
    }

    public string GenerateRefreshToken()
    {
        var randomNumber = new byte[64];
        using var randomNumberGenerator = RandomNumberGenerator.Create();

        randomNumberGenerator.GetBytes(randomNumber);

        return Convert.ToBase64String(randomNumber);
    }

    public void WriteAuthTokenAsHttpOnlyCookie(string cookieName, string token, DateTime expiration)
    {
        var httpContext = _httpContextAccessor.HttpContext;

        if (httpContext == null)
        {
            throw new InvalidOperationException("HttpContext is not available. This method must be called within an HTTP request context.");
        }

        var cookieOptions = new CookieOptions
                            {
                                Secure = true,
                                HttpOnly = true,
                                Expires = expiration,
                                IsEssential = true,
                                SameSite = SameSiteMode.Strict
                            };
        httpContext.Response.Cookies.Append(cookieName, token, cookieOptions);
    }
}
