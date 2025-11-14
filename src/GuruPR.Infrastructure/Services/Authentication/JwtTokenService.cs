using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

using GuruPR.Application.Common.Interfaces.Infrastructure;
using GuruPR.Application.Dtos.Jwt;
using GuruPR.Application.Settings.Authentication;
using GuruPR.Domain.Entities;
using GuruPR.Infrastructure.Exceptions;
using GuruPR.Infrastructure.Identity.Constants;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace GuruPR.Infrastructure.Services.Authentication;

public class JwtTokenService : ITokenService
{
    private readonly ILogger<JwtTokenService> _logger;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IHasher _hasher;
    private readonly UserManager<User> _userManager;
    private readonly JwtSettings _jwtSettings;
    private readonly RefreshTokenSettings _refreshTokenSettings;

    public JwtTokenService(ILogger<JwtTokenService> logger,
                           IHttpContextAccessor httpContextAccessor,
                           IHasher hasher,
                           UserManager<User> userManager,
                           IOptions<JwtSettings> jwtSettings,
                           IOptions<RefreshTokenSettings> refreshTokenSettings)
    {
        _logger = logger;
        _httpContextAccessor = httpContextAccessor;
        _hasher = hasher;
        _userManager = userManager;
        _jwtSettings = jwtSettings.Value;
        _refreshTokenSettings = refreshTokenSettings.Value;
    }

    #region Public Methods

    public async Task IssueNewTokenPairAsync(User user)
    {
        ArgumentNullException.ThrowIfNull(user);

        var jwtTokenResult = await GenerateAccessTokenAsync(user);
        var refreshToken = GenerateRefreshToken();
        var refreshTokenExpiry = DateTime.UtcNow.AddDays(_refreshTokenSettings.ExpirationTimeInDays);

        user.UpdateRefreshToken(_hasher.Hash(refreshToken), refreshTokenExpiry);

        var updateResult = await _userManager.UpdateAsync(user);
        if (!updateResult.Succeeded)
        {
            var errors = string.Join(", ", updateResult.Errors.Select(e => e.Description));
            _logger.LogError("Failed to update refresh token for user {UserId}. Errors: {Errors}", user.Id, errors);

            throw new TokenUpdateFailedException("Failed to update refresh token for the user.");
        }

        WriteAccessTokenCookie(jwtTokenResult.Token, jwtTokenResult.ExpiresAtUtc);
        WriteRefreshTokenCookie(refreshToken, refreshTokenExpiry);
    }

    public async Task RenewAccessTokenAsync(User user)
    {
        ArgumentNullException.ThrowIfNull(user);

        var jwtTokenResult = await GenerateAccessTokenAsync(user);

        WriteAccessTokenCookie(jwtTokenResult.Token, jwtTokenResult.ExpiresAtUtc);
    }

    public async Task RevokeTokensAsync(User user)
    {
        ArgumentNullException.ThrowIfNull(user);

        user.ClearRefreshToken();

        var updateResult = await _userManager.UpdateAsync(user);
        if (!updateResult.Succeeded)
        {
            var errors = string.Join(", ", updateResult.Errors.Select(error => error.Description));
            _logger.LogError("Failed to revoke tokens for user {UserId}. Errors: {Errors}", user.Id, errors);

            throw new TokenUpdateFailedException("Failed to revoke tokens for the user.");
        }

        ClearAuthenticationCookies();
    }

    #endregion

    #region Private Methods - Token Generation

    private async Task<JwtTokenResult> GenerateAccessTokenAsync(User user)
    {
        var userRoles = await _userManager.GetRolesAsync(user);
        var claims = BuildClaims(user, userRoles);
        var signingCredentials = CreateSigningCredentials();
        var expiresAt = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpirationTimeInMinutes);
        var token = new JwtSecurityToken(issuer: _jwtSettings.Issuer,
                                         audience: _jwtSettings.Audience,
                                         claims: claims,
                                         notBefore: DateTime.UtcNow,
                                         expires: expiresAt,
                                         signingCredentials: signingCredentials);
        var jwtToken = new JwtSecurityTokenHandler().WriteToken(token);

        return new JwtTokenResult
        {
            Token = jwtToken,
            ExpiresAtUtc = expiresAt
        };
    }

    private List<Claim> BuildClaims(User user, IList<string> roles)
    {
        var claims = new List<Claim>
                     {
                         new(JwtClaimTypes.Subject, user.Id.ToString()),
                         new(JwtClaimTypes.JwtId, Guid.NewGuid().ToString()),
                         new(JwtClaimTypes.Email, user.Email ?? string.Empty),
                         new(JwtClaimTypes.Name, user.ToString())
                     };

        claims.AddRange(roles.Select(role => new Claim(JwtClaimTypes.Role, role)));

        return claims;
    }

    private SigningCredentials CreateSigningCredentials()
    {
        var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey));

        return new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);
    }

    private static string GenerateRefreshToken()
    {
        var randomBytes = RandomNumberGenerator.GetBytes(64);

        return Convert.ToBase64String(randomBytes);
    }

    #endregion

    #region Private Methods - Cookie Management

    private void WriteAccessTokenCookie(string token, DateTime expiration)
    {
        var httpContext = GetHttpContext();
        var cookieOptions = CreateSecureCookieOptions(expiration);

        httpContext.Response.Cookies.Append("AccessToken", token, cookieOptions);
    }

    private void WriteRefreshTokenCookie(string token, DateTime expiration)
    {
        var httpContext = GetHttpContext();
        var cookieOptions = CreateSecureCookieOptions(expiration);

        httpContext.Response.Cookies.Append("RefreshToken", token, cookieOptions);
    }

    private void ClearAuthenticationCookies()
    {
        var httpContext = GetHttpContext();

        httpContext.Response.Cookies.Delete("AccessToken");
        httpContext.Response.Cookies.Delete("RefreshToken");
    }

    private HttpContext GetHttpContext()
    {
        return _httpContextAccessor.HttpContext ??
            throw new InvalidOperationException("HttpContext is not available. This method must be called within an HTTP request context.");
    }

    private CookieOptions CreateSecureCookieOptions(DateTime expiration)
    {
        return new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict, // Use Strict for better security, change to None only if needed for CORS
            IsEssential = true,
            Expires = expiration,
            // Path = "/"
            // Domain = _jwtSettings.CookieDomain // Configure in production
        };
    }

    #endregion
}
