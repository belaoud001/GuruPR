namespace GuruPR.Application.Exceptions.Account;

public class UntrustedReturnUrlException(string returnUrl) : Exception($"The return URL '{returnUrl}' is not trusted.");
