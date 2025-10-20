namespace GuruPR.Application.Exceptions.Account;

public interface IValidationException
{
    public IReadOnlyDictionary<string, List<string>> Errors { get; }
}
