using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

using GuruPR.Application.Dtos.Jwt;
using GuruPR.Application.Interfaces.Infrastructure;
using GuruPR.Application.Settings.Security;
using GuruPR.Domain.Entities;
using GuruPR.Infrastructure.Identity.Constants;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace GuruPR.Infrastructure.Services.Authentication;

public class JwtTokenService : ITokenService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly UserManager<User> _userManager;
    private readonly JwtSettings _jwtSettings;

    public JwtTokenService(IHttpContextAccessor httpContextAccessor, UserManager<User> userManager, IOptions<JwtSettings> jwtSettings)
    {
        _httpContextAccessor = httpContextAccessor;
        _userManager = userManager;
        _jwtSettings = jwtSettings.Value;
    }

    public async Task<JwtTokenResult> GenerateTokenAsync(User user)
    {
        var userRoles = await _userManager.GetRolesAsync(user);
        var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey));
        var signingCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);
        var claims = new List<Claim>
        {
            new Claim(JwtClaimTypes.Subject, user.Id.ToString()),
            new Claim(JwtClaimTypes.JwtId,   Guid.NewGuid().ToString()),
            new Claim(JwtClaimTypes.Email,   user.Email ?? string.Empty),

            new Claim(JwtClaimTypes.Name, user.ToString())
        };
        claims.AddRange(userRoles.Select(role => new Claim(JwtClaimTypes.Role, role)));

        JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();


        var expires = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpirationTimeInMinutes);
        var token = new JwtSecurityToken(issuer: _jwtSettings.Issuer,
                                         audience: _jwtSettings.Audience,
                                         claims: claims,
                                         notBefore: DateTime.UtcNow,
                                         expires: expires,
                                         signingCredentials: signingCredentials);
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
            SameSite = SameSiteMode.None
        };
        httpContext.Response.Cookies.Append(cookieName, token, cookieOptions);
    }
}
