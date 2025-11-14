using GuruPR.Application.Common.Exceptions;

namespace GuruPR.Application.Exceptions.Account;

public class RegistrationFailedException : ValidationExceptionBase
{
    public RegistrationFailedException(string message) : base(message)
    {
    }

    public RegistrationFailedException(string message, IReadOnlyDictionary<string, List<string>> errors) : base(message, errors)
    {
    }
}
