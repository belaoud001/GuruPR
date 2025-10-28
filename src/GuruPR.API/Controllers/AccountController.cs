using GuruPR.Application.Interfaces.Application;
using GuruPR.Application.Interfaces.Infrastructure;
using GuruPR.Domain.Requests;
using GuruPR.Infrastructure.Identity.Constants;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GuruPR.Controllers;

[Authorize]
[ApiController]
[Route("api/v1/accounts")]
public class AccountController : ControllerBase
{
    private readonly ILogger<AccountController> _logger;
    private readonly IAccountService _accountService;
    private readonly IExternalAuthService _externalAuthService;

    public AccountController(ILogger<AccountController> logger,
                             IAccountService accountService,
                             IExternalAuthService externalAuthService)
    {
        _logger = logger;
        _accountService = accountService;
        _externalAuthService = externalAuthService;
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
            var redirectUrl = await _accountService.GetEmailConfirmationRedirectUrlAsync(true);

            return Redirect(redirectUrl);
        }
        catch (Exception)
        {
            var redirectUrl = await _accountService.GetEmailConfirmationRedirectUrlAsync(false);

            return Redirect(redirectUrl);
        }
    }

    [HttpGet("login/google")]
    [AllowAnonymous]
    public async Task<IActionResult> GoogleLoginAsync([FromQuery] string? returnUrl)
    {
        var challengeResult = await _externalAuthService.InitiateGoogleLoginAsync(returnUrl, HttpContext);

        return challengeResult;
    }

    [HttpGet("login/google/callback", Name = "GoogleLoginCallback")]
    [AllowAnonymous]
    public async Task<IActionResult> GoogleLoginCallbackAsync([FromQuery] string returnUrl)
    {
        var redirectUrl = await _externalAuthService.HandleGoogleCallbackAsync(returnUrl, HttpContext);

        return Redirect(redirectUrl);
    }

    [HttpPost("logout")]
    public async Task<IActionResult> LogoutAsync([FromBody] LogoutRequest logoutRequest)
    {
        await _accountService.LogoutAsync(logoutRequest.UserId, logoutRequest.RefreshToken);

        return Ok("Logout has succeeded.");
    }
}
