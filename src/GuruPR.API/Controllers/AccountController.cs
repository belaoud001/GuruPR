using GuruPR.Application.Interfaces.Application;
using GuruPR.Domain.Entities;
using GuruPR.Domain.Requests;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace GuruPR.Controllers;

[ApiController]
[Route("api/v1/accounts")]
public class AccountController : ControllerBase
{
    private readonly ILogger<AccountController> _logger;
    private readonly IAccountService _accountService;
    private readonly UserManager<User> _userManager;

    public AccountController(ILogger<AccountController> logger, IAccountService accountService)
    {
        _logger = logger;
        _accountService = accountService;
    }

    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<IActionResult> RegisterAsync(RegisterRequest registerRequest)
    {
        await _accountService.RegisterAsync(registerRequest);

        return Ok("Registration succeeded.");
    }

    [HttpPost("login")]
    public async Task<IActionResult> LoginAsync(LoginRequest loginRequest)
    {
        await _accountService.LoginAsync(loginRequest);

        return Ok("Login was successful.");
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> RefreshTokenAsync(string refrehToken)
    {
        await _accountService.RefreshTokenAsync(refrehToken);

        return Ok("Token refresh has succeeded.");
    }

    [HttpGet("ConfirmEmail")]
    public async Task<IActionResult> ConfirmEmail(string userId, string token)
    {
        await _accountService.ConfirmEmailAsync(userId, token);

        return Ok("Email confirmation has succeeded.");
    }

    [HttpPost("logout")]
    public async Task<IActionResult> LogoutAsync()
    {
        return Ok("Logout endpoint is under construction.");
    }
}
