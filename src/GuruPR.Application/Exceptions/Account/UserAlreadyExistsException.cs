namespace GuruPR.Application.Exceptions.Account;

public class UserAlreadyExistsException(string message) : AccountException(message);
