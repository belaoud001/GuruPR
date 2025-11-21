using FluentValidation;

namespace GuruPR.Application.Common.Extensions.Validation;

public static class ValidationRulesExtensions
{
    public static IRuleBuilderOptions<T, string> ValidId<T>(this IRuleBuilder<T, string> ruleBuilder, string domain)
    {
        return ruleBuilder.NotEmpty()
                          .WithMessage($"{domain} Id is required.");
    }

    public static IRuleBuilderOptions<T, string> ValidAbsoluteUrl<T>(this IRuleBuilder<T, string> ruleBuilder, string url)
    {
        return ruleBuilder.NotEmpty()
                          .WithMessage($"{url} is required.")
                          .Must(url => Uri.IsWellFormedUriString(url, UriKind.Absolute))
                          .WithMessage($"{url} must be a valid absolute URL.");
    }
}
