using GuruPR.Application.Common.Exceptions;

namespace GuruPR.Application.Features.Account.Exceptions;

public class EmailSendFailedException(string message) : OperationFailedException(message);
