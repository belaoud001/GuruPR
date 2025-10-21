using GuruPR.Application.Exceptions;
using GuruPR.Application.Exceptions.Account;
using GuruPR.Application.Interfaces.Application;
using GuruPR.Application.Interfaces.Infrastructure;
using GuruPR.Application.Interfaces.Persistence;
using GuruPR.Domain.Entities;
using GuruPR.Domain.Requests;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Routing;

namespace GuruPR.Application.Services.Account;

public class AccountService : IAccountService
{
    private readonly ITokenService _tokenService;
    private readonly IUserRepository _userRepository;
    private readonly IEmailSender _emailSender;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly LinkGenerator _linkGenerator;
    private readonly UserManager<User> _userManager;

    private const int RefreshTokenExpirationDays = 7;

    public AccountService(ITokenService tokenService, 
                          IUserRepository userRepository, 
                          IEmailSender emailSender,
                          IHttpContextAccessor httpContextAccessor,
                          LinkGenerator linkGenerator,
                          UserManager<User> userManager)
    {
        _tokenService = tokenService ?? throw new ArgumentNullException(nameof(tokenService));
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _emailSender = emailSender ?? throw new ArgumentNullException(nameof(emailSender));
        _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        _linkGenerator = linkGenerator ?? throw new ArgumentNullException(nameof(linkGenerator));
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
        else
        {
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var httpContext = _httpContextAccessor.HttpContext ?? throw new OperationFailedException("Email confirmation failed.");

            var confirmationLink = _linkGenerator.GetUriByAction(httpContext,
                                                                 action: "ConfirmEmail",
                                                                 controller: "Account",
                                                                 values: new { userId = user.Id, token = token });

            await _emailSender.SendEmailAsync(user.Email, 
                                              "Confirm your email",
                                              $"Please confirm your account by clicking <a href='{confirmationLink}'>here</a>.");
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

    public async Task ConfirmEmailAsync(string userId, string token)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            throw new UserNotFoundException($"User with the specified ID {userId} was not found.");
        }

        var result = await _userManager.ConfirmEmailAsync(user, token);
        if (!result.Succeeded)
        {
            throw new EmailConfirmationException("Email confirmation failed.");
        }
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