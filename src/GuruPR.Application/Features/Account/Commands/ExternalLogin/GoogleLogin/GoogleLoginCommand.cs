using Microsoft.AspNetCore.Mvc;

namespace GuruPR.Application.Features.Account.Commands.ExternalLogin.GoogleLogin;

public record GoogleLoginCommand(string ReturnUrl) : ExternalLoginCommand<ChallengeResult>(ReturnUrl);
