namespace GuruPR.Application.Exceptions.Account;

public class UntrustedReturnUrlException(string returnUrl) : AccountException($"The return URL '{returnUrl}' is not trusted.");
