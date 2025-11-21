using System.Collections.ObjectModel;

using GuruPR.Application.Common.Exceptions;

using Microsoft.AspNetCore.Identity;

namespace GuruPR.Application.Common.Extensions;

public static class IdentityErrorsExtensions
{
    public static void ThrowValidationException<TException>(this IEnumerable<IdentityError> errors, string message) where TException : ValidationExceptionBase
    {
        var errorGroups = errors.GroupBy(error => GetErrorCategory(error.Code))
                                .ToDictionary(
                                    group => group.Key,
                                    group => group.Select(error => error.Description).ToList()
                                );

        var readonlyErrors = new ReadOnlyDictionary<string, List<string>>(errorGroups);
        var exception = Activator.CreateInstance(typeof(TException), message, readonlyErrors) as TException
                        ?? throw new InvalidOperationException(
                            $"Type {typeof(TException).Name} must have a constructor (string message, IReadOnlyDictionary<string, List<string>> errors)."
                        );

        throw exception;
    }

    private static string GetErrorCategory(string errorCode)
    {
        if (errorCode.StartsWith("Password", StringComparison.OrdinalIgnoreCase))
        {
            return "Password";
        }

        if (errorCode.Contains("Email", StringComparison.OrdinalIgnoreCase))
        {
            return "Email";
        }

        if (errorCode.StartsWith("FirstName", StringComparison.OrdinalIgnoreCase))
        {
            return "FirstName";
        }

        if (errorCode.StartsWith("LastName", StringComparison.OrdinalIgnoreCase))
        {
            return "LastName";
        }

        return errorCode;
    }
}
