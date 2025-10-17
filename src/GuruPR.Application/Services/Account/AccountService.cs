using Microsoft.AspNetCore.Identity;

using GuruPR.Domain.Entities;
using GuruPR.Domain.Requests;
using GuruPR.Application.Exceptions;
using GuruPR.Application.Interfaces.Application;
using GuruPR.Application.Interfaces.Infrastructure;

namespace GuruPR.Application.Services.Account;

public class AccountService : IAccountService
{
    private readonly ITokenService _tokenService;
    private readonly UserManager<User> _userManager;
    private const int RefreshTokenExpirationDays = 7;

    public AccountService(ITokenService tokenService, UserManager<User> userManager)
    {
        _tokenService = tokenService ?? throw new ArgumentNullException(nameof(tokenService));
        _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
    }

    public async Task RegisterAsync(RegisterRequest registerRequest)
    {
        if (registerRequest == null)
        {
            throw new ArgumentNullException(nameof(registerRequest));
        }

        await EnsureUserDoesNotExistAsync(registerRequest.Email);

        var user = new User
        {
            FirstName = registerRequest.FirstName,
            LastName = registerRequest.LastName,
            Email = registerRequest.Email,
            UserName = registerRequest.Email
        };

        var result = await _userManager.CreateAsync(user, registerRequest.Password);


        if (!result.Succeeded) 
        { 
            throw new AccountException($"Failed to create a new user with email '{registerRequest.Email}'.");
        }
    }

    public async Task LoginAsync(LoginRequest loginRequest)
    {
        if (loginRequest == null)
        {
            throw new ArgumentNullException(nameof(loginRequest));
        }

        var user = await FindUserByEmailAsync(loginRequest.Email);

        if (!await _userManager.CheckPasswordAsync(user, loginRequest.Password))
        {
            throw new AccountException("Invalid email or password.");
        }

        await SetAuthenticationTokensAsync(user);
    }

    public async Task RefreshTokenAsync(string refreshToken)
    {
        if (string.IsNullOrWhiteSpace(refreshToken))
        {
            throw new AccountException("Refresh token is missing.");
        }

        var user = FindUserByRefreshToken(refreshToken);

        if (user.RefreshTokenExpiryTime <= DateTime.UtcNow)
        {
            throw new AccountException("Refresh token has expired.");
        }

        await SetAuthenticationTokensAsync(user);
    }

    private async Task<User?> EnsureUserDoesNotExistAsync(string email)
    {
        var user = await _userManager.FindByEmailAsync(email);

        if (user != null)
        {
            throw new AccountException($"User with email '{email}' already exists.");
        }

        return user;
    }

    private async Task<User> FindUserByEmailAsync(string email)
    {
        var user = await _userManager.FindByEmailAsync(email);

        if (user == null)
        {
            throw new AccountException("Invalid email or password.");
        }

        return user;
    }

    private User FindUserByRefreshToken(string refreshToken)
    {
        var user = _userManager.Users.SingleOrDefault(u => u.RefreshToken == refreshToken);

        if (user == null)
        {
            throw new AccountException("Invalid refresh token.");
        }

        return user;
    }

    private async Task SetAuthenticationTokensAsync(User user)
    {
        var jwtTokenResult = _tokenService.GenerateToken(user);
        var newRefreshToken = _tokenService.GenerateRefreshToken();
        var refreshTokenExpiry = DateTime.UtcNow.AddDays(RefreshTokenExpirationDays);

        user.RefreshToken = newRefreshToken;
        user.RefreshTokenExpiryTime = refreshTokenExpiry;

        var updateResult = await _userManager.UpdateAsync(user);

        if (!updateResult.Succeeded)
        {
            throw new AccountException($"Failed to update user with email {user.Email}");
        }

        _tokenService.WriteAuthTokenAsHttpOnlyCookie("AccessToken", jwtTokenResult.Token, jwtTokenResult.ExpiresAtUtc);
        _tokenService.WriteAuthTokenAsHttpOnlyCookie("RefreshToken", newRefreshToken, refreshTokenExpiry);
    }
}