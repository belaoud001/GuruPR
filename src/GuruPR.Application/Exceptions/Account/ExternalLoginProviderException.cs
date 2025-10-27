namespace GuruPR.Application.Exceptions.Account;

public class ExternalLoginProviderException(string provider, string message)
    : Exception($"{provider} provider error: {message}");
