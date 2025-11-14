namespace GuruPR.Application.Features.Account.Commands.ExternalLogin.GoogleCallback;

public record GoogleCallbackCommand(string ReturnUrl) : ExternalLoginCommand<string>(ReturnUrl);
