using GuruPR.Application.Interfaces.Application;
using GuruPR.Application.Settings.FrontEnd;
using GuruPR.Domain.Entities;
using GuruPR.Domain.Enums;
using GuruPR.Domain.Extensions.Auth;
using GuruPR.Domain.Requests;
using GuruPR.Infrastructure.Identity.Constants;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace GuruPR.Controllers;

[Authorize]
[ApiController]
[Route("api/v1/accounts")]
public class AccountController : ControllerBase
{
    private readonly ILogger<AccountController> _logger;
    private readonly IAccountService _accountService;
    private readonly IUrlValidator _urlValidator;
    private readonly LinkGenerator _linkGenerator;
    private readonly SignInManager<User> _signInManager;
    private readonly FrontEndSettings _frontEndSettings;

    public AccountController(ILogger<AccountController> logger,
                             IAccountService accountService,
                             IUrlValidator urlValidator,
                             LinkGenerator linkGenerator,
                             SignInManager<User> signInManager,
                             IOptions<FrontEndSettings> frontEndSettings)
    {
        _logger = logger;
        _accountService = accountService;
        _urlValidator = urlValidator;
        _linkGenerator = linkGenerator;
        _signInManager = signInManager;
        _frontEndSettings = frontEndSettings.Value;
    }

    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<IActionResult> RegisterAsync([FromBody] RegisterRequest registerRequest)
    {
        await _accountService.RegisterAsync(registerRequest);

        return Ok("Registration succeeded.");
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> LoginAsync([FromBody] LoginRequest loginRequest)
    {
        await _accountService.LoginAsync(loginRequest);

        return Ok("Login was successful.");
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> RefreshTokenAsync([FromBody] RefreshRequest refreshRequest)
    {
        var userId = User.Claims.FirstOrDefault(claim => claim.Type == JwtClaimTypes.Subject)?.Value;

        if (userId == null)
        {
            return Unauthorized("Invalid token or missing subject claim.");
        }

        await _accountService.RefreshTokenAsync(userId, refreshRequest.RefreshToken);

        return Ok("Token refresh has succeeded.");
    }

    [HttpGet("confirm-email", Name = "ConfirmEmail")]
    [AllowAnonymous]
    public async Task<IActionResult> ConfirmEmailAsync(string userId, string token)
    {
        try
        {
            await _accountService.ConfirmEmailAsync(userId, token);

            // Redirect to login page
            return Redirect(_frontEndSettings.BaseUrl + _frontEndSettings.EmailConfirmationPath);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Email confirmation failed for userId: {UserId}", userId);

            return Redirect(_frontEndSettings.BaseUrl + _frontEndSettings.EmailConfirmationFailedPath);
        }
    }

    [HttpGet("login/google")]
    [AllowAnonymous]
    public IActionResult GoogleLogin([FromQuery] string? returnUrl)
    {
        _urlValidator.ValidateReturnUrl(returnUrl);

        var callbackUrl = _linkGenerator.GetUriByName(HttpContext, "GoogleLoginCallback", new { returnUrl });
        if (string.IsNullOrEmpty(callbackUrl))
        {
            return BadRequest("Unable to generate callback URL");
        }

        var properties = _signInManager.ConfigureExternalAuthenticationProperties("Google", callbackUrl);

        return new ChallengeResult("Google", properties);
    }

    [HttpGet("login/google/callback", Name = "GoogleLoginCallback")]
    [AllowAnonymous]
    public async Task<IActionResult> GoogleLoginCallbackAsync([FromQuery] string returnUrl)
    {
        _urlValidator.ValidateReturnUrl(returnUrl);

        var result = await HttpContext.AuthenticateAsync(GoogleDefaults.AuthenticationScheme);
        if (!result.Succeeded)
        {
            return Unauthorized("Google authentication failed.");
        }

        var provider = ExternalProvider.Google.ToName();
        await _accountService.LoginWithExternalProviderAsync(result.Principal, provider);

        return Redirect(returnUrl);
    }

    [HttpPost("logout")]
    public async Task<IActionResult> LogoutAsync([FromBody] LogoutRequest logoutRequest)
    {
        await _accountService.LogoutAsync(logoutRequest.UserId, logoutRequest.RefreshToken);

        return Ok("Logout has succeeded.");
    }
}
