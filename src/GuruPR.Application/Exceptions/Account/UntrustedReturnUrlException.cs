namespace GuruPR.Application.Exceptions.Account;

public class UntrustedReturnUrlException : Exception
{
    public UntrustedReturnUrlException(string returnUrl) : base($"The return URL '{returnUrl}' is not trusted.")
    {
    }
}
