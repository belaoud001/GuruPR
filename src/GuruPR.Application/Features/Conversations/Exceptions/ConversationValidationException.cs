using GuruPR.Application.Common.Exceptions;

namespace GuruPR.Application.Features.Conversations.Exceptions;

public class ConversationValidationException : ValidationExceptionBase
{
    public ConversationValidationException(string message) : base(message)
    {
    }

    public ConversationValidationException(string message, IReadOnlyDictionary<string, List<string>> errors) : base(message, errors)
    {
    }
}
