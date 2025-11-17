namespace GuruPR.Application.Features.Account.Commands.ExternalLogin.Google.GoogleCallback;

public record GoogleCallbackCommand(string? ReturnUrl) : ExternalLoginCommand<string>(ReturnUrl);
