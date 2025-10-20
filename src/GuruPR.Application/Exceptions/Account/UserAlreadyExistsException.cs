namespace GuruPR.Application.Exceptions.Account;

public class UserAlreadyExistsException : AccountException
{
    public UserAlreadyExistsException(string message) : base(message)
    {
    }
}
