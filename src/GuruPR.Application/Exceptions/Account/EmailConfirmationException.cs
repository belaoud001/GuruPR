namespace GuruPR.Application.Exceptions.Account;

public class EmailConfirmationException : Exception
{
    public EmailConfirmationException(string message) : base(message)
    {
    }
}
