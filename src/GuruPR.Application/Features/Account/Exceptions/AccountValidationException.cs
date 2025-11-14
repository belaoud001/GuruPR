using GuruPR.Application.Common.Exceptions;

namespace GuruPR.Application.Features.Account.Exceptions;

public class AccountValidationException : ValidationExceptionBase
{
    public AccountValidationException(string message) : base(message)
    {
    }

    public AccountValidationException(string message, IReadOnlyDictionary<string, List<string>> errors) : base(message, errors)
    {
    }
}
