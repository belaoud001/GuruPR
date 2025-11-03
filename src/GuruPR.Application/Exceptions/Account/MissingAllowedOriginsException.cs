namespace GuruPR.Application.Exceptions.Account;

public class MissingAllowedOriginsException(string message) : AccountException(message);
