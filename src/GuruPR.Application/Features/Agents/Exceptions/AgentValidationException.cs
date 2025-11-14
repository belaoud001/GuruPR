using GuruPR.Application.Common.Exceptions;

namespace GuruPR.Application.Features.Agents.Exceptions;

public class AgentValidationException : ValidationExceptionBase
{
    public AgentValidationException(string message) : base(message)
    {
    }

    public AgentValidationException(string message, IReadOnlyDictionary<string, List<string>> errors) : base(message, errors)
    {
    }
}
