using GuruPR.Application.Exceptions.Account;

namespace GuruPR.Application.Features.Account.Exceptions;

public class LoginFailedException(string message) : AccountException(message);
