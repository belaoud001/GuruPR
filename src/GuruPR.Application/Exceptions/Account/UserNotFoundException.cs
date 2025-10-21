namespace GuruPR.Application.Exceptions.Account;

public class UserNotFoundException : NotFoundException
{
    public UserNotFoundException(string message) : base(message)
    {
    }
}
