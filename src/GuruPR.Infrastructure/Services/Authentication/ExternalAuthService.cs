using GuruPR.Application.Common.Interfaces.Application;
using GuruPR.Application.Common.Interfaces.Infrastructure;
using GuruPR.Domain.Entities;
using GuruPR.Domain.Enums;
using GuruPR.Domain.Extensions.Authentication;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;

namespace GuruPR.Infrastructure.Services.Authentication;

public class ExternalAuthService : IExternalAuthService
{
    private readonly ILogger<ExternalAuthService> _logger;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IExternalUserProvisioningService _externalUserProvisioningService;
    private readonly LinkGenerator _linkGenerator;
    private readonly SignInManager<User> _signInManager;

    public ExternalAuthService(ILogger<ExternalAuthService> logger,
                               IHttpContextAccessor httpContextAccessor,
                               IExternalUserProvisioningService externalUserProvisioningService,
                               LinkGenerator linkGenerator,
                               SignInManager<User> signInManager)
    {
        _logger = logger;
        _httpContextAccessor = httpContextAccessor;
        _externalUserProvisioningService = externalUserProvisioningService;
        _linkGenerator = linkGenerator;
        _signInManager = signInManager;
    }

    public Task<ChallengeResult> InitiateGoogleLoginAsync(string? returnUrl)
    {
        try
        {
            var httpContext = _httpContextAccessor.HttpContext ?? throw new InvalidOperationException("Http context is missing.");

            var callbackUrl = _linkGenerator.GetUriByName(httpContext, "GoogleLoginCallback", new { returnUrl });
            if (string.IsNullOrEmpty(callbackUrl))
            {
                _logger.LogError("Failed to generate Google callback URL");

                throw new InvalidOperationException("Unable to generate callback URL");
            }

            var properties = _signInManager.ConfigureExternalAuthenticationProperties("Google", callbackUrl);
            var challengeResult = new ChallengeResult("Google", properties);

            return Task.FromResult(challengeResult);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error initiating Google login flow");

            throw;
        }
    }

    public async Task<string> HandleGoogleCallbackAsync(string returnUrl)
    {
        try
        {
            var httpContext = _httpContextAccessor.HttpContext ?? throw new InvalidOperationException("Http context is missing.");

            var authResult = await httpContext.AuthenticateAsync(GoogleDefaults.AuthenticationScheme);
            if (!authResult.Succeeded)
            {
                _logger.LogWarning("Google authentication failed. AuthResult: {AuthResult}", authResult);

                throw new UnauthorizedAccessException("Google authentication failed.");
            }

            if (authResult.Principal == null)
            {
                _logger.LogWarning("Google authentication succeeded but principal is null");

                throw new UnauthorizedAccessException("Google authentication failed - no principal.");
            }

            var provider = ExternalProvider.Google.ToName();
            await _externalUserProvisioningService.LoginWithExternalProviderAsync(authResult.Principal, provider);

            return returnUrl;
        }
        catch (UnauthorizedAccessException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling Google login callback");

            throw;
        }
    }
}
