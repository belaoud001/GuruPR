using Microsoft.AspNetCore.Identity;

using GuruPR.Domain.Entities;
using GuruPR.Domain.Requests;
using GuruPR.Application.Exceptions.Account;
using GuruPR.Application.Interfaces.Application;
using GuruPR.Application.Interfaces.Persistence;
using GuruPR.Application.Interfaces.Infrastructure;

namespace GuruPR.Application.Services.Account;

public class AccountService : IAccountService
{
    private readonly ITokenService _tokenService;
    private readonly IUserRepository _userRepository;
    private readonly UserManager<User> _userManager;

    private const int RefreshTokenExpirationDays = 7;

    public AccountService(ITokenService tokenService, IUserRepository userRepository, UserManager<User> userManager)
    {
        _tokenService = tokenService ?? throw new ArgumentNullException(nameof(tokenService));
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
    }

    #region Public Methods

    public async Task RegisterAsync(RegisterRequest registerRequest)
    {
        ArgumentNullException.ThrowIfNull(registerRequest, nameof(registerRequest));

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
            ThrowRegistrationException(result.Errors);
        }
    }

    public async Task LoginAsync(LoginRequest loginRequest)
    {
        ArgumentNullException.ThrowIfNull(loginRequest, nameof(loginRequest));

        var user = await FindUserByEmailAsync(loginRequest.Email);

        if (!await _userManager.CheckPasswordAsync(user, loginRequest.Password))
        {
            throw new LoginFailedException("Login failed. Invalid email or password.");
        }

        await SetAuthenticationTokensAsync(user);
    }

    public async Task RefreshTokenAsync(string refreshToken)
    {
        if (string.IsNullOrWhiteSpace(refreshToken))
        {
            throw new RefreshTokenException("Refresh token is missing.");
        }

        var user = await FindUserByRefreshTokenAsync(refreshToken);

        if (user.RefreshTokenExpiryTime <= DateTime.UtcNow)
        {
            throw new RefreshTokenException("Refresh token has expired.");
        }

        await SetAuthenticationTokensAsync(user);
    }

    #endregion

    #region Private Methods

    private static void ThrowRegistrationException(IEnumerable<IdentityError> errors)
    {
        var errorGroups = errors.GroupBy(error => GetErrorCategory(error.Code))
                                .ToDictionary(
                                    group => group.Key,
                                    group => group.Select(error => error.Description)
                                                  .ToList()
                                );

        throw new RegistrationFailedException($"Registration failed.", errorGroups);
    }

    private static string GetErrorCategory(string errorCode)
    {
        if (errorCode.StartsWith("Password", StringComparison.OrdinalIgnoreCase))
        {
            return "Password";
        }

        if (errorCode.Contains("Email", StringComparison.OrdinalIgnoreCase))
        {
            return "Email";
        }

        if (errorCode.StartsWith("FirstName", StringComparison.OrdinalIgnoreCase))
        {
            return "FirstName";
        }

        if (errorCode.StartsWith("LastName", StringComparison.OrdinalIgnoreCase))
        {
            return "LastName";
        }

        return "Other";
    }

    private async Task<User?> EnsureUserDoesNotExistAsync(string email)
    {
        var user = await _userManager.FindByEmailAsync(email);

        if (user != null)
        {
            throw new UserAlreadyExistsException($"User with email '{email}' already exists.");
        }

        return user;
    }

    private async Task<User> FindUserByEmailAsync(string email)
    {
        var user = await _userManager.FindByEmailAsync(email);

        return user ?? throw new LoginFailedException("Invalid email or password.");
    }

    private async Task<User> FindUserByRefreshTokenAsync(string refreshToken)
    {
        var user = await _userRepository.GetUserByRefreshTokenAsync(refreshToken);

        return user ?? throw new RefreshTokenException("Unable to retrieve user for refresh token.");
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

    #endregion
}