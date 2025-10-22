using GuruPR.Application.Interfaces.Application;
using GuruPR.Application.Settings.FrontEnd;
using GuruPR.Domain.Requests;

using Microsoft.AspNetCore.Authorization;
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
    private readonly FrontEndSettings _frontEndSettings;

    public AccountController(ILogger<AccountController> logger, IAccountService accountService, IOptions<FrontEndSettings> frontEndSettings)
    {
        _logger = logger;
        _accountService = accountService;
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
    public async Task<IActionResult> RefreshTokenAsync([FromBody] string refrehToken)
    {
        await _accountService.RefreshTokenAsync(refrehToken);

        return Ok("Token refresh has succeeded.");
    }

    [HttpGet("confirm-email")]
    [AllowAnonymous]
    public async Task<IActionResult> ConfirmEmail(string userId, string token)
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

    [HttpPost("logout")]
    public async Task<IActionResult> LogoutAsync([FromBody] LogoutRequest logoutRequest)
    {
        await _accountService.LogoutAsync(logoutRequest.UserId, logoutRequest.RefreshToken);

        return Ok("Logout has succeeded.");
    }
}
