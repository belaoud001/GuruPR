using FluentValidation;

using GuruPR.Application.Common.Extensions.Validation;

namespace GuruPR.Application.Features.ProviderConnections.Validators.Extensions;

public static class ProviderConnectionValidationRules
{
    public static IRuleBuilderOptions<T, string> ValidProviderId<T>(this IRuleBuilder<T, string> ruleBuilder)
    {
        return ruleBuilder.ValidId("Provider");
    }

    public static IRuleBuilderOptions<T, string> ValidProviderConnectionId<T>(this IRuleBuilder<T, string> ruleBuilder)
    {
        return ruleBuilder.ValidId("Provider Connection");
    }

    public static IRuleBuilderOptions<T, string> ValidClientId<T>(this IRuleBuilder<T, string> ruleBuilder)
    {
        return ruleBuilder.NotEmpty()
                          .WithMessage("Client Id is required.");
    }

    public static IRuleBuilderOptions<T, string> ValidClientSecret<T>(this IRuleBuilder<T, string> ruleBuilder)
    {
        return ruleBuilder.NotEmpty()
                          .WithMessage("Client Secret is required.");
    }

    public static IRuleBuilderOptions<T, List<string>> ValidScopes<T>(this IRuleBuilder<T, List<string>> ruleBuilder)
    {
        return ruleBuilder.NotEmpty()
                          .WithMessage("At least one scope is required.");
    }
}
