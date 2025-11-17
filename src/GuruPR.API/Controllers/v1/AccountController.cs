using Asp.Versioning;

using GuruPR.Application.Features.Account.Commands.ConfirmEmail;
using GuruPR.Application.Features.Account.Commands.ExternalLogin.Google.GoogleCallback;
using GuruPR.Application.Features.Account.Commands.ExternalLogin.Google.GoogleLogin;
using GuruPR.Application.Features.Account.Commands.Login;
using GuruPR.Application.Features.Account.Commands.Logout;
using GuruPR.Application.Features.Account.Commands.RefreshToken;
using GuruPR.Application.Features.Account.Commands.Register;

using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GuruPR.Controllers.v1;

[Authorize]
[ApiController]
[ApiVersion(1.0)]
[Route("api/v{version:apiVersion}/accounts")]
public class AccountController : ControllerBase
{
    private readonly ILogger<AccountController> _logger;
    private readonly IMediator _mediator;

    public AccountController(ILogger<AccountController> logger,
                             IMediator mediator)
    {
        _logger = logger;
        _mediator = mediator;
    }

    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<IActionResult> RegisterAsync([FromBody] RegisterCommand registerCommand)
    {
        await _mediator.Send(registerCommand);

        return Ok("Registration succeeded.");
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> LoginAsync([FromBody] LoginCommand loginCommand)
    {
        await _mediator.Send(loginCommand);

        return Ok("Login was successful.");
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> RefreshTokenAsync([FromBody] RefreshTokenCommand refreshTokenCommand)
    {
        await _mediator.Send(refreshTokenCommand);

        return Ok("Token refresh has succeeded.");
    }

    [HttpGet("confirm-email", Name = "ConfirmEmail")]
    [AllowAnonymous]
    public async Task<IActionResult> ConfirmEmailAsync(string userId, string token)
    {
        await _mediator.Send(new ConfirmEmailCommand(userId, token));

        return Ok("Email confirmation succeeded.");
    }

    [HttpGet("login/google")]
    [AllowAnonymous]
    public async Task<IActionResult> GoogleLoginAsync([FromQuery] string? returnUrl)
    {
        var challengeResult = await _mediator.Send(new GoogleLoginCommand(returnUrl));

        return challengeResult;
    }

    [HttpGet("login/google/callback", Name = "GoogleLoginCallback")]
    [AllowAnonymous]
    public async Task<IActionResult> GoogleLoginCallbackAsync([FromQuery] string returnUrl)
    {
        var redirectUrl = await _mediator.Send(new GoogleCallbackCommand(returnUrl));

        return Redirect(redirectUrl);
    }

    [HttpPost("logout")]
    public async Task<IActionResult> LogoutAsync([FromBody] LogoutCommand logoutCommand)
    {
        await _mediator.Send(logoutCommand);

        return Ok("Logout has succeeded.");
    }
}
