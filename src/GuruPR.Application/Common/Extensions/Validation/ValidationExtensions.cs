using FluentValidation;

using GuruPR.Application.Common.Exceptions;

namespace GuruPR.Application.Common.Extensions.Validation;

public static class ValidationExtensions
{
    public static async Task ThrowIfInvalidAsync<TValidator, TInstance, TException>(this TValidator validator,
                                                                                         TInstance instance,
                                                                                         Func<string, IReadOnlyDictionary<string, List<string>>, TException> exceptionFactory,
                                                                                         CancellationToken cancellationToken = default)
           where TValidator : IValidator<TInstance>
           where TException : ValidationExceptionBase
    {
        var result = await validator.ValidateAsync(instance, cancellationToken);

        if (!result.IsValid)
        {
            var errorDictionary = result.Errors.GroupBy(validationFailure => validationFailure.PropertyName)
                                               .ToDictionary(group => group.Key,
                                                             group => group.Select(validationFailure => validationFailure.ErrorMessage)
                                                                           .ToList());

            throw exceptionFactory("Validation failed.", errorDictionary);
        }
    }

    public static async Task ThrowIfInvalidAsync<TValidator, TInstance>(this TValidator validator,
                                                                             TInstance instance,
                                                                             CancellationToken cancellationToken = default)
           where TValidator : IValidator<TInstance>
    {
        await validator.ThrowIfInvalidAsync(instance,
                                            (message, errors) => new ValidationExceptionBase(message, errors),
                                            cancellationToken);
    }
}
