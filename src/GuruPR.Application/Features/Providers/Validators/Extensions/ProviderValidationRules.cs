using FluentValidation;

using GuruPR.Application.Common.Extensions.Validation;

namespace GuruPR.Application.Features.Providers.Validators.Extensions;

public static class ProviderValidationRules
{
    public static IRuleBuilderOptions<T, string> ValidProviderId<T>(this IRuleBuilder<T, string> ruleBuilder)
    {
        return ruleBuilder.ValidId("Provider");
    }



}
