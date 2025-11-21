using GuruPR.Application.Exceptions.Account;

namespace GuruPR.Application.Features.Account.Exceptions;

public class EmailConfirmationException(string message) : AccountException(message);
