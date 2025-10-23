namespace GuruPR.Application.Exceptions.Account;

public class ExternalLoginProviderException : Exception
{
    public ExternalLoginProviderException(string provider, string message) : base($"{provider} provider error: {message}")
    {
    }
}
