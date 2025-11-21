using FluentValidation;

using GuruPR.Application.Features.Account.Validators.Extensions;

namespace GuruPR.Application.Features.Account.Commands.RefreshToken;

public class RefreshTokenValidator : AbstractValidator<RefreshTokenCommand>
{
    public RefreshTokenValidator()
    {
        RuleFor(command => command.RefreshToken).ValidRefreshToken();
    }
}
