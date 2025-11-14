using FluentValidation;

namespace GuruPR.Application.Features.Account.Validators.Extensions;

public static class AccountValidationRules
{
    public static IRuleBuilderOptions<T, string> ValidFirstName<T>(this IRuleBuilder<T, string> ruleBuilder)
    {
        return ruleBuilder.NotEmpty()
                          .WithMessage("First name is required.")
                          .MinimumLength(2)
                          .WithMessage("First name must be at least 2 characters long.");
    }

    public static IRuleBuilderOptions<T, string> ValidLastName<T>(this IRuleBuilder<T, string> ruleBuilder)
    {
        return ruleBuilder.NotEmpty()
                          .WithMessage("Last name is required.")
                          .MinimumLength(2)
                          .WithMessage("Last name must be at least 2 characters long.");
    }

    public static IRuleBuilderOptions<T, string> ValidEmail<T>(this IRuleBuilder<T, string> ruleBuilder)
    {
        return ruleBuilder.NotEmpty()
                          .WithMessage("Email is required.");
    }

    public static IRuleBuilderOptions<T, string> ValidPassword<T>(this IRuleBuilder<T, string> ruleBuilder)
    {
        return ruleBuilder.NotEmpty()
                          .WithMessage("Password is required.")
                          .MinimumLength(8)
                          .WithMessage("Password must be at least 8 characters long.");
    }

    public static IRuleBuilderOptions<T, string> ValidRefreshToken<T>(this IRuleBuilder<T, string> ruleBuilder)
    {
        return ruleBuilder.NotEmpty()
                          .WithMessage("Refresh token is required.");
    }
}
