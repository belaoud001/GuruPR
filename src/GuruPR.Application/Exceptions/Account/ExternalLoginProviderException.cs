namespace GuruPR.Application.Exceptions.Account;

public class ExternalLoginProviderException(string provider, string message)
    : AccountException($"{provider} provider error: {message}");
