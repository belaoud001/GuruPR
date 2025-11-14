using GuruPR.Application.Exceptions.Account;

namespace GuruPR.Application.Features.Account.Exceptions;

public class LogoutException(string message) : AccountException(message);
