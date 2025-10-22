namespace GuruPR.Application.Exceptions.Account;

public class UserRoleOperationFailedException : Exception
{
    public UserRoleOperationFailedException(string message) : base(message)
    {
    }
}
