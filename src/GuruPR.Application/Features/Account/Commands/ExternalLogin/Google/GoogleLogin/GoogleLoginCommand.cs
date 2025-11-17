using Microsoft.AspNetCore.Mvc;

namespace GuruPR.Application.Features.Account.Commands.ExternalLogin.Google.GoogleLogin;

public record GoogleLoginCommand(string? ReturnUrl) : ExternalLoginCommand<ChallengeResult>(ReturnUrl);
