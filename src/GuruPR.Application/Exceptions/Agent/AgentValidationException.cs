
using GuruPR.Application.Exceptions.Interfaces;

namespace GuruPR.Application.Exceptions.Agent;

public class AgentValidationException : AgentException, IValidationException
{
    public IReadOnlyDictionary<string, List<string>> Errors { get; }

    public AgentValidationException(string message) : base(message)
    {
        Errors = new Dictionary<string, List<string>>();
    }

    public AgentValidationException(string message, IReadOnlyDictionary<string, List<string>> errors) : base(message)
    {
        Errors = errors;
    }
}
