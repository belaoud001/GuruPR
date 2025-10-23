namespace GuruPR.Application.Exceptions.Account;

public class InvalidReturnUrlException : Exception
{
    public InvalidReturnUrlException(string message) : base(message)
    {
    }
}
