using GuruPR.Application.Common.Exceptions;

namespace GuruPR.Application.Features.Account.Exceptions;

public class RegistrationValidationException : ValidationExceptionBase
{
    public RegistrationValidationException(string message) : base(message)
    {
    }

    public RegistrationValidationException(string message, IReadOnlyDictionary<string, List<string>> errors) : base(message)
    {
    }
}
