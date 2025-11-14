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
                catch (InvalidReturnUrlException)
                {
                    context.MessageFormatter.AppendArgument("Error", "Return URL format is invalid.");
                }
                catch (UntrustedReturnUrlException)
                {
                    context.MessageFormatter.AppendArgument("Error", "Return URL is not from a trusted origin.");
                }
                catch (MissingAllowedOriginsException)
                {
                    context.MessageFormatter.AppendArgument("Error", "Allowed origins are not configured.");
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
