using FluentValidation;

using GuruPR.Application.Features.Account.Validators.Extensions;

namespace GuruPR.Application.Features.Account.Commands.Login;

public class LoginValidator : AbstractValidator<LoginCommand>
{
    public LoginValidator()
    {
        RuleFor(loginCommand => loginCommand.Email).ValidEmail();

        RuleFor(loginCommand => loginCommand.Password).ValidPassword();
    }
}
