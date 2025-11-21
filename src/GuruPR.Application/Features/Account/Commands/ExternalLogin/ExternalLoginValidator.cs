using FluentValidation;

using GuruPR.Application.Common.Interfaces.Application;
using GuruPR.Application.Exceptions.Account;

namespace GuruPR.Application.Features.Account.Commands.ExternalLogin;

public class ExternalLoginValidator<T> : AbstractValidator<ExternalLoginCommand<T>>
{
    public ExternalLoginValidator(IUrlValidator urlValidator)
    {
        RuleFor(externalLoginCommand => externalLoginCommand.ReturnUrl)
            .NotEmpty()
            .WithMessage("Return URL is required.")
            .Must((command, returnUrl, context) =>
            {
                try
                {
                    urlValidator.ValidateReturnUrl(returnUrl);
                    return true;
                }
                catch (InvalidReturnUrlException exception)
                {
                    context.MessageFormatter.AppendArgument("Error", exception.Message);
                }
                catch (UntrustedReturnUrlException exception)
                {
                    context.MessageFormatter.AppendArgument("Error", exception.Message);
                }
                catch (MissingAllowedOriginsException exception)
                {
                    context.MessageFormatter.AppendArgument("Error", exception.Message);
                }
                catch (Exception)
                {
                    context.MessageFormatter.AppendArgument("Error", "Unexpected error occurred while validating Return URL.");
                }

                return false;
            })
            .WithMessage("{Error}");
    }
}
