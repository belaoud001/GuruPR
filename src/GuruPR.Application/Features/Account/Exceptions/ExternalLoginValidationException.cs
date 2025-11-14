using GuruPR.Application.Common.Exceptions;

namespace GuruPR.Application.Features.Account.Exceptions;

public class ExternalLoginValidationException : ValidationExceptionBase
{
    public ExternalLoginValidationException(string message) : base(message)
    {
    }

    public ExternalLoginValidationException(string message, IReadOnlyDictionary<string, List<string>> errors) : base(message, errors)
    {
    }
}
