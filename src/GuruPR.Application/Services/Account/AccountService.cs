using System.Text.Encodings.Web;

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
    private readonly IUnitOfWork _unitOfWork;
    private readonly IEmailSender _emailSender;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly LinkGenerator _linkGenerator;
    private readonly UserManager<User> _userManager;

    private const int RefreshTokenExpirationDays = 7;

    public AccountService(ITokenService tokenService, 
                          IUnitOfWork unitOfWork,
                          IEmailSender emailSender,
                          IHttpContextAccessor httpContextAccessor,
                          LinkGenerator linkGenerator,
                          UserManager<User> userManager)
    {
        _tokenService = tokenService ?? throw new ArgumentNullException(nameof(tokenService));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
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

        await _unitOfWork.BeginUserManagementTransactionAsync();

        try
        {
            var user = await CreateUserAsync(registerRequest);
            await SendConfirmationEmailAsync(user);

            await _unitOfWork.CommitUserManagementTransactionAsync();
        }
        catch (Exception)
        {
            await _unitOfWork.RollbackUserManagementTransactionAsync();

            throw;
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
        var user = await _unitOfWork.Users.GetUserByRefreshTokenAsync(refreshToken);

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
        var confirmationLink = GenerateConfirmationLink(user.Id, token);
        var emailBody = BuildConfirmationEmailBody(user.FirstName, confirmationLink);

        await _emailSender.SendEmailAsync(user.Email ?? throw new OperationFailedException("Failed to send confirmation email."),
                                          "Confirm Your GuruPR Account ✨",
                                          emailBody);
    }

    private string GenerateConfirmationLink(Guid userId, string token)
    {
        var httpContext = _httpContextAccessor.HttpContext ?? throw new OperationFailedException("Unable to generate confirmation link.");

        var confirmationLink = _linkGenerator.GetUriByAction(httpContext,
                                                             action: "ConfirmEmail",
                                                             controller: "Account",
                                                             values: new { userId = userId, token });

        return confirmationLink ?? throw new OperationFailedException("Failed to generate confirmation link.");
    }

    private static string BuildConfirmationEmailBody(string firstName, string confirmationLink)
    {
        return $@"
                <div style=""font-family:'Inter', 'Noto Sans JP', Arial, sans-serif; color:#111; background-color:#f7f7f7; padding:40px 0;"">
                  <div style=""max-width:600px; margin:0 auto; background-color:#fff; padding:40px 50px; border:1px solid #e0e0e0;"">
    
                    <h1 style=""color:#111; font-weight:500; font-size:26px; margin-bottom:15px;"">
                      Welcome to <span style=""color:#1E40AF;"">GuruPR</span>
                    </h1>

                    <p style=""color:#333; font-size:16px; line-height:1.6; margin-bottom:25px;"">
                      Hi <strong>{HtmlEncoder.Default.Encode(firstName)}</strong>,
                    </p>

                    <p style=""color:#555; font-size:15px; line-height:1.6; margin-bottom:35px;"">
                      Thank you for joining <strong>GuruPR</strong>. Please confirm your email to activate your account and start exploring.
                    </p>

                    <div style=""text-align:center; margin-bottom:40px;"">
                      <a href=""{HtmlEncoder.Default.Encode(confirmationLink)}""
                         style=""background-color:#1E40AF; color:#fff; text-decoration:none; 
                                padding:12px 28px; font-weight:500; font-size:15px; 
                                display:inline-block;"">
                        Confirm Email
                      </a>
                    </div>

                    <p style=""color:#777; font-size:14px; line-height:1.6; margin-bottom:30px;"">
                      If you didn’t create an account with GuruPR, you can safely ignore this message.
                    </p>

                    <hr style=""border:none; border-top:1px solid #e0e0e0; margin:30px 0;"" />

                    <p style=""font-size:12px; color:#999; text-align:center;"">
                      © {DateTime.UtcNow.Year} GuruPR. All rights reserved.
                    </p>

                    <p style=""font-size:12px; color:#aaa; text-align:center;"">
                      Made with 💡 by the GuruPR team
                    </p>
                  </div>
                </div>";
    }

    #endregion
}