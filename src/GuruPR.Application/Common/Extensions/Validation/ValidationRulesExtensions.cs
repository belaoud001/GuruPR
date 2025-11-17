using FluentValidation;

namespace GuruPR.Application.Common.Extensions.Validation;

public static class ValidationRulesExtensions
{
    public static IRuleBuilderOptions<T, string> ValidId<T>(this IRuleBuilder<T, string> ruleBuilder, string domain)
    {
        return ruleBuilder.NotNull()
                          .NotEmpty()
                          .WithMessage($"{domain} Id is required.");
    }
}
