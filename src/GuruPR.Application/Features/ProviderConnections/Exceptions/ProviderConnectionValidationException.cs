using GuruPR.Application.Common.Exceptions;

namespace GuruPR.Application.Features.ProviderConnections.Exceptions;

public class ProviderConnectionValidationException : ValidationExceptionBase
{
    public ProviderConnectionValidationException(string message) : base(message)
    {
    }

    public ProviderConnectionValidationException(string message, IReadOnlyDictionary<string, List<string>> errors) : base(message, errors)
    {
    }
}
