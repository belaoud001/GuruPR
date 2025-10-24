using System.Security.Claims;
using System.Text.Encodings.Web;

using GuruPR.Application.Exceptions;
using GuruPR.Application.Exceptions.Account;
using GuruPR.Application.Interfaces.Application;
using GuruPR.Application.Interfaces.Infrastructure;
using GuruPR.Application.Interfaces.Persistence;
using GuruPR.Application.Services.Email;
using GuruPR.Domain.Entities;
using GuruPR.Domain.Enums;
using GuruPR.Domain.Extensions.Auth;
using GuruPR.Domain.Extensions.User;
using GuruPR.Domain.Requests;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;

namespace GuruPR.Application.Services.Account;

public class AccountService : IAccountService
{
    private readonly ILogger<AccountService> _logger;
    private readonly ITokenService _tokenService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IEmailSender _emailSender;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IEmailTemplateService _emailTemplateService;
    private readonly IAccountLinkGenerator _accountLinkGenerator;
    private readonly LinkGenerator _linkGenerator;
    private readonly UserManager<User> _userManager;

    private const int RefreshTokenExpirationDays = 7;

    public AccountService(ILogger<AccountService> logger,
                          ITokenService tokenService,
                          IUnitOfWork unitOfWork,
                          IEmailSender emailSender,
                          IEmailTemplateService emailTemplateService,
                          IAccountLinkGenerator accountLinkGenerator,
                          IHttpContextAccessor httpContextAccessor,
                          LinkGenerator linkGenerator,
                          UserManager<User> userManager)
    {
        _logger = logger;
        _tokenService = tokenService;
        _unitOfWork = unitOfWork;
        _emailSender = emailSender;
        _emailTemplateService = emailTemplateService;
        _accountLinkGenerator = accountLinkGenerator;
        _httpContextAccessor = httpContextAccessor;
        _linkGenerator = linkGenerator;
        _userManager = userManager;
    }

    #region Public Methods

    public async Task RegisterAsync(RegisterRequest registerRequest)
    {
        ArgumentNullException.ThrowIfNull(registerRequest, nameof(registerRequest));

        await EnsureUserDoesNotExistAsync(registerRequest.Email);

        await _unitOfWork.BeginUserManagementTransactionAsync();

        User? user = null;

        try
        {
            user = await CreateUserAsync(registerRequest);

            await AssignRoleAsync(user.Id.ToString(), UserRole.User);

            await _unitOfWork.CommitUserManagementTransactionAsync();
        }
        catch (Exception)
        {
            await _unitOfWork.RollbackUserManagementTransactionAsync();

            throw;
        }

        await SendConfirmationEmailAsync(user);
    }

    public async Task LoginAsync(LoginRequest loginRequest)
    {
        ArgumentNullException.ThrowIfNull(loginRequest, nameof(loginRequest));

        var user = await FindUserByEmailAsync(loginRequest.Email);

        if (user == null)
        {
            throw new UserNotFoundException($"User with the specified email {loginRequest.Email} was not found.");
        }

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
        var user = await GetUserByIdOrThrowException(userId);

        var result = await _userManager.ConfirmEmailAsync(user, token);
        if (!result.Succeeded)
        {
            throw new EmailConfirmationException("Email confirmation failed.");
        }
    }

    public async Task LogoutAsync(string userId, string refreshToken)
    {
        var user = await GetUserByIdOrThrowException(userId);

        if (user.RefreshToken != refreshToken  || user.RefreshTokenExpiryTime > DateTime.UtcNow)
        {
            throw new RefreshTokenException("Invalid refresh token or user ID.");
        }

        user.RefreshToken = null;

        var updateResult = await _userManager.UpdateAsync(user);
        if (!updateResult.Succeeded)
        {
            throw new LogoutException($"Failed to logout user with email {user.Email}");
        }

        _httpContextAccessor.HttpContext?.Response.Cookies.Delete("AccessToken");
        _httpContextAccessor.HttpContext?.Response.Cookies.Delete("RefreshToken");
    }

    public async Task AssignRoleAsync(string userId, UserRole userRole)
    {
        var user = await GetUserByIdOrThrowException(userId);

        var result = await _userManager.AddToRoleAsync(user, userRole.ToName());
        if (!result.Succeeded)
        {
            throw new UserRoleOperationFailedException($"Failed to add role {userRole.ToName()} to user with email {user.Email}");
        }
    }

    public async Task RemoveRoleAsync(string userId, UserRole userRole)
    {
        var user = await GetUserByIdOrThrowException(userId);

        var result = await _userManager.RemoveFromRoleAsync(user, userRole.ToName());
        if (!result.Succeeded)
        {
            throw new UserRoleOperationFailedException($"Failed to remove role {userRole.ToName()} from user with email {user.Email}");
        }
    }

    public async Task LoginWithExternalProviderAsync(ClaimsPrincipal? claimsPrincipal, string provider)
    {
        if (claimsPrincipal == null)
        {
            throw new ExternalLoginProviderException(provider, "Claims principal is missing");
        }

        var email = ExtractEmailFromClaims(claimsPrincipal, provider);
        var user = await _userManager.FindByEmailAsync(email);

        await _unitOfWork.BeginUserManagementTransactionAsync();

        try
        {
            if (user == null)
            {
                user = await CreateUserFromExternalProviderClaimsAsync(claimsPrincipal, provider, email);
            }

            await SetAuthenticationTokensAsync(user);

            await _unitOfWork.CommitUserManagementTransactionAsync();
        }
        catch (Exception)
        {
            await _unitOfWork.RollbackUserManagementTransactionAsync();

            throw;
        }
    }

    #endregion

    #region Private Methods

    private async Task<User> GetUserByIdOrThrowException(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            throw new UserNotFoundException($"User with the specified ID {userId} was not found.");
        }

        return user;
    }

    private void ThrowRegistrationException(IEnumerable<IdentityError> errors)
    {
        var errorGroups = errors.GroupBy(error => GetErrorCategory(error.Code))
                                .ToDictionary(
                                    group => group.Key,
                                    group => group.Select(error => error.Description)
                                                  .ToList()
                                );

        throw new RegistrationFailedException($"Registration failed.", errorGroups);
    }

    private string GetErrorCategory(string errorCode)
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

        return null;
    }

    private async Task<User> FindUserByEmailAsync(string email)
    {
        var user = await _userManager.FindByEmailAsync(email);

        return user ?? throw new LoginFailedException("Invalid email or password.");
    }

    private async Task<User> FindUserByRefreshTokenAsync(string refreshToken)
    {
        var user = await _unitOfWork.Users.GetUserByRefreshTokenAsync(refreshToken);

        return user ?? throw new RefreshTokenException("Unable to retrieve user for refresh token.");
    }

    private async Task SetAuthenticationTokensAsync(User user)
    {
        var jwtTokenResult = await _tokenService.GenerateTokenAsync(user);
        var newRefreshToken = _tokenService.GenerateRefreshToken();
        var refreshTokenExpiry = DateTime.UtcNow.AddDays(RefreshTokenExpirationDays);

        user.RefreshToken = newRefreshToken;
        user.RefreshTokenExpiryTime = refreshTokenExpiry;

        var updateResult = await _userManager.UpdateAsync(user);

        if (!updateResult.Succeeded)
        {
            throw new OperationFailedException($"Failed to update tokens for user with email {user.Email}.");
        }

        _tokenService.WriteAuthTokenAsHttpOnlyCookie("AccessToken", jwtTokenResult.Token, jwtTokenResult.ExpiresAtUtc);
        _tokenService.WriteAuthTokenAsHttpOnlyCookie("RefreshToken", newRefreshToken, refreshTokenExpiry);
    }

    private async Task<User> CreateUserAsync(RegisterRequest registerRequest)
    {
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

        return user;
    }

    private async Task SendConfirmationEmailAsync(User user)
    {
        var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        var confirmationLink = _accountLinkGenerator.GenerateConfirmationLink(user.Id, token);
        var emailBody = _emailTemplateService.BuildConfirmationEmailBody(user.FirstName, confirmationLink);

        await _emailSender.SendEmailAsync(user.Email ?? throw new OperationFailedException("Failed to send confirmation email."),
                                          "Confirm Your GuruPR Account ✨",
                                          emailBody);
    }

    private static string ExtractEmailFromClaims(ClaimsPrincipal claimsPrincipal, string provider)
    {
        var email = claimsPrincipal.FindFirstValue(ClaimTypes.Email);
        if (string.IsNullOrWhiteSpace(email))
        {
            throw new ExternalLoginProviderException(provider, "Email claim is missing");
        }

        return email;
    }

    private async Task<User> CreateUserFromExternalProviderClaimsAsync(ClaimsPrincipal claimsPrincipal, string provider, string email)
    {
        var user = new User
        {
            Email = email,
            UserName = email,
            FirstName = claimsPrincipal.FindFirstValue(ClaimTypes.GivenName) ?? provider.Normalize(),
            LastName = claimsPrincipal.FindFirstValue(ClaimTypes.Surname) ?? "User",
            EmailConfirmed = true
        };

        var createResult = await _userManager.CreateAsync(user);
        if (!createResult.Succeeded)
        {
            var errors = string.Join(", ", createResult.Errors.Select(e => e.Description));

            _logger.LogError("Error creating user from external provider {Provider}: {Errors}", provider, errors);

            throw new RegistrationFailedException($"Failed to create a user account using the external provider {provider}.");
        }

        await AssignRoleAsync(user.Id.ToString(), UserRole.User);
        await AddLoginInfoAsync(user, provider, claimsPrincipal);

        return user;
    }

    private async Task AddLoginInfoAsync(User user, string provider, ClaimsPrincipal claimsPrincipal)
    {
        var userNameIdentifier = claimsPrincipal.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrWhiteSpace(userNameIdentifier))
        {
            throw new ExternalLoginProviderException(provider, $"{provider} user ID claim is missing");
        }

        var loginInfo = new UserLoginInfo(provider, userNameIdentifier, provider);
        var loginResult = await _userManager.AddLoginAsync(user, loginInfo);

        if (!loginResult.Succeeded)
        {
            var errors = string.Join(", ", loginResult.Errors.Select(e => e.Description));
            throw new ExternalLoginProviderException(provider, $"Unable to link {provider} login: {errors}");
        }
    }

    #endregion
}