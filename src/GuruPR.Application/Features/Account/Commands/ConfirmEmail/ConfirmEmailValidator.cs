using FluentValidation;

namespace GuruPR.Application.Features.Account.Commands.ConfirmEmail;

public class ConfirmEmailValidator : AbstractValidator<ConfirmEmailCommand>
{
    public ConfirmEmailValidator()
    {
        RuleFor(command => command.UserId).NotEmpty()
                                          .WithMessage("User ID must not be empty.");

        RuleFor(command => command.Token).NotEmpty()
                                         .WithMessage("Token must not be empty.");
    }
}
