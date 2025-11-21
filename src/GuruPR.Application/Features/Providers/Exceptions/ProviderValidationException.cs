using GuruPR.Application.Common.Exceptions;

namespace GuruPR.Application.Features.Providers.Exceptions;

public class ProviderValidationException : ValidationExceptionBase
{
    public ProviderValidationException(string message) : base(message)
    {
    }

    public ProviderValidationException(string message, IReadOnlyDictionary<string, List<string>> errors) : base(message, errors)
    {
    }
}
