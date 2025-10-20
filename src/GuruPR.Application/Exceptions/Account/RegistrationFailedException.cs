namespace GuruPR.Application.Exceptions.Account;

public class RegistrationFailedException : AccountException, IValidationException
{
    public IReadOnlyDictionary<string, List<string>> Errors { get; }

    public RegistrationFailedException(string message) : base(message)
    {
        Errors = new Dictionary<string, List<string>>();
    }

    public RegistrationFailedException(string message, IReadOnlyDictionary<string, List<string>> errors) : base(message)
    {
        Errors = errors;
    }
}
