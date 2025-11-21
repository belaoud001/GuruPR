using FluentValidation;

using GuruPR.Application.Features.Account.Validators.Extensions;

namespace GuruPR.Application.Features.Account.Commands.Logout;

public class LogoutValidator : AbstractValidator<LogoutCommand>
{
    public LogoutValidator()
    {
        RuleFor(logoutCommand => logoutCommand.RefreshToken).ValidRefreshToken();
    }
}
