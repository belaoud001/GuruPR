using GuruPR.Application.Common.Exceptions.Interfaces;

namespace GuruPR.Application.Common.Exceptions;

public class ValidationExceptionBase : Exception, IValidationException
{
    public IReadOnlyDictionary<string, List<string>> Errors { get; }

    public ValidationExceptionBase(string message) : base(message)
    {
        Errors = new Dictionary<string, List<string>>();
    }

    public ValidationExceptionBase(string message, IReadOnlyDictionary<string, List<string>> errors) : base(message)
    {
        Errors = errors ?? new Dictionary<string, List<string>>();
    }
}
