using FluentValidation;

using GuruPR.Domain.Entities.Agents.Configurations;

namespace GuruPR.Application.Features.Agents.Validators.Extensions;

public static class AgentValidationRules
{
    public static IRuleBuilderOptions<T, string> ValidAgentName<T>(this IRuleBuilder<T, string> ruleBuilder)
    {
        return ruleBuilder.NotEmpty()
                          .WithMessage("Name is required.");
    }

    public static IRuleBuilderOptions<T, string> ValidAvatarUrl<T>(this IRuleBuilder<T, string> ruleBuilder)
    {
        return ruleBuilder.NotEmpty()
                          .WithMessage("AvatarUrl is required.")
                          .Must(url => Uri.IsWellFormedUriString(url, UriKind.Absolute))
                          .WithMessage("AvatarUrl must be a valid absolute URL.");
    }

    public static IRuleBuilderOptions<T, string> ValidDescription<T>(this IRuleBuilder<T, string> ruleBuilder)
    {
        return ruleBuilder.NotEmpty()
                          .WithMessage("Description is required.")
                          .MinimumLength(5)
                          .WithMessage("Description must be at least 5 characters long.");
    }

    public static IRuleBuilderOptions<T, ModelConfiguration> ValidModelConfiguration<T>(this IRuleBuilder<T, ModelConfiguration> ruleBuilder)
    {
        return ruleBuilder.NotNull()
                          .WithMessage("Model configuration is required.")
                          .SetValidator(new ModelConfigurationValidator());
    }

    public static IRuleBuilderOptions<T, MemoryConfiguration> ValidMemoryConfiguration<T>(this IRuleBuilder<T, MemoryConfiguration> ruleBuilder)
    {
        return ruleBuilder.SetValidator(new MemoryConfigurationValidator())
                          .When(memoryConfiguration => memoryConfiguration != null);
    }
}
