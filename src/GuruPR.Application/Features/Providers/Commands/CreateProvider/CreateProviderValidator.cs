using FluentValidation;

using GuruPR.Application.Features.Providers.Validators.Extensions;

namespace GuruPR.Application.Features.Providers.Commands.CreateProvider;

public class CreateProviderValidator : AbstractValidator<CreateProviderCommand>
{
    public CreateProviderValidator()
    {
        RuleFor(command => command.DisplayName).ValidDisplayName();

        RuleFor(command => command.TokenUrl).ValidTokenUrl();

        RuleFor(command => command.ProviderType).ValidProviderType();

        RuleFor(command => command.AuthorizationUrl).ValidAuthorizationUrl();
    }
}
