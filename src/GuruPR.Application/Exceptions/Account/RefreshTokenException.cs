namespace GuruPR.Application.Exceptions.Account;

public class RefreshTokenException : AccountException
{
    public RefreshTokenException(string message) : base(message)
    {
    }
}
