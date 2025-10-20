using Microsoft.AspNetCore.Mvc;

using GuruPR.Domain.Requests;
using GuruPR.Application.Interfaces.Application;
using Microsoft.AspNetCore.Authorization;

namespace GuruPR.Controllers;

[ApiController]
[Route("api/v1/accounts")]
public class AccountController : ControllerBase
{
    private readonly ILogger<AccountController> _logger;
    private readonly IAccountService _accountService;

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

    [HttpPost("logout")]
    public async Task<IActionResult> LogoutAsync()
    {
        return Ok("Logout endpoint is under construction.");
    }
}
