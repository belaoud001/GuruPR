using System.Security.Claims;

using GuruPR.Application.Common.Interfaces.Application;
using GuruPR.Application.Common.Interfaces.Infrastructure;
using GuruPR.Application.Common.Interfaces.Persistence;
using GuruPR.Application.Exceptions.Account;
using GuruPR.Domain.Entities;
using GuruPR.Domain.Enums;

using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace GuruPR.Application.Common.Services;

public class ExternalUserProvisioningService : IExternalUserProvisioningService
{
    private readonly ILogger<ExternalUserProvisioningService> _logger;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITokenService _tokenService;
    private readonly IUserRoleService _userRoleService;
    private readonly IExternalUserFactory _externalUserFactory;
    private readonly UserManager<User> _userManager;

    public ExternalUserProvisioningService(ILogger<ExternalUserProvisioningService> logger,
                                           IUnitOfWork unitOfWork,
                                           ITokenService tokenService,
                                           IUserRoleService userRoleService,
                                           IExternalUserFactory externalUserFactory,
                                           UserManager<User> userManager)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
        _tokenService = tokenService;
        _userRoleService = userRoleService;
        _externalUserFactory = externalUserFactory;
        _userManager = userManager;
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

            await _unitOfWork.CommitUserManagementTransactionAsync();
            await _tokenService.IssueNewTokenPairAsync(user);
        }
        catch (Exception)
        {
            await _unitOfWork.RollbackUserManagementTransactionAsync();

            throw;
        }
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
        var user = _externalUserFactory.Create(claimsPrincipal, provider, email);

        var createResult = await _userManager.CreateAsync(user);
        if (!createResult.Succeeded)
        {
            var errors = string.Join(", ", createResult.Errors.Select(e => e.Description));

            _logger.LogError("Error creating user from external provider {Provider}: {Errors}", provider, errors);

            throw new RegistrationFailedException($"Failed to create a user account using the external provider {provider}.");
        }

        await _userRoleService.AssignRoleAsync(user.Id.ToString(), UserRole.User);
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
}
