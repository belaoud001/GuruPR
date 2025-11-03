
using GuruPR.Application.Exceptions.Interfaces;

namespace GuruPR.Application.Exceptions.Agent;

public class AgentValidationException : Exception, IValidationException
{
    public IReadOnlyDictionary<string, List<string>> Errors { get; }

    public AgentValidationException(string message)
    {
        Errors = new Dictionary<string, List<string>>();
    }

    public AgentValidationException(string message, IReadOnlyDictionary<string, List<string>> errors)
    {
        Errors = errors;
    }
}
