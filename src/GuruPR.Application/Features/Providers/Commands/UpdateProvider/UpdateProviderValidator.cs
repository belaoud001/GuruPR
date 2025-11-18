using FluentValidation;

using GuruPR.Application.Features.Providers.Validators.Extensions;
using GuruPR.Domain.Entities.Provider.Enums;

namespace GuruPR.Application.Features.Providers.Commands.UpdateProvider;

public class UpdateProviderValidator : AbstractValidator<UpdateProviderCommand>
{
    public UpdateProviderValidator()
    {
        RuleFor(command => command.DisplayName).ValidDisplayName();

        RuleFor(command => command.TokenUrl).ValidTokenUrl();

        RuleFor(command => command.ProviderType ?? OAuthProviderType.Other).ValidProviderType();

        RuleFor(command => command.AuthorizationUrl).ValidAuthorizationUrl();
    }
}
