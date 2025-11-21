using GuruPR.Application.Common.Exceptions;

namespace GuruPR.Application.Features.Conversations.Exceptions;

public class ConversationNotFoundException(string message) : NotFoundException(message);
