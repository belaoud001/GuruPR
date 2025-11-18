using FluentValidation;

using GuruPR.Application.Common.Extensions.Validation;
using GuruPR.Domain.Entities.Provider.Enums;

namespace GuruPR.Application.Features.Providers.Validators.Extensions;

public static class ProviderValidationRules
{
    public static IRuleBuilderOptions<T, string> ValidProviderId<T>(this IRuleBuilder<T, string> ruleBuilder)
    {
        return ruleBuilder.ValidId("Provider");
    }

    public static IRuleBuilderOptions<T, string> ValidDisplayName<T>(this IRuleBuilder<T, string> ruleBuilder)
    {
        return ruleBuilder.NotEmpty()
                          .MinimumLength(3)
                          .WithMessage("Display name must be at least 3 characters long.");
    }

    public static IRuleBuilderOptions<T, OAuthProviderType> ValidProviderType<T>(this IRuleBuilder<T, OAuthProviderType> ruleBuilder)
    {
        return ruleBuilder.NotEmpty()
                          .WithMessage("Provider type is required.");
    }

    public static IRuleBuilderOptions<T, string> ValidTokenUrl<T>(this IRuleBuilder<T, string> ruleBuilder)
    {
        return ruleBuilder.ValidAbsoluteUrl("Token url");
    }

    public static IRuleBuilderOptions<T, string> ValidAuthorizationUrl<T>(this IRuleBuilder<T, string> ruleBuilder)
    {
        return ruleBuilder.ValidAbsoluteUrl("Authorization url");
    }
}
