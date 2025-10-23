namespace GuruPR.Application.Exceptions.Account;

public class MissingAllowedOriginsException : Exception
{
    public MissingAllowedOriginsException(string message) : base(message)
    {
    }
}
