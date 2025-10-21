namespace GuruPR.Application.Exceptions.Account;

public class LoginFailedException : Exception
{
    public LoginFailedException(string message) : base(message)
    {
    }
}
