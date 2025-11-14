using FluentValidation;

using GuruPR.Application.Features.Account.Validators.Extensions;

namespace GuruPR.Application.Features.Account.Commands.Register;

public class RegisterValidator : AbstractValidator<RegisterCommand>
{
    public RegisterValidator()
    {
        RuleFor(registerCommand => registerCommand.FirstName).ValidFirstName();

        RuleFor(registerCommand => registerCommand.LastName).ValidLastName();

        RuleFor(registerCommand => registerCommand.Email).ValidEmail();

        RuleFor(registerCommand => registerCommand.Password).ValidEmail();
    }
}
