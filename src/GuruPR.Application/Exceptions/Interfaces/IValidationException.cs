namespace GuruPR.Application.Exceptions.Interfaces;

public interface IValidationException
{
    public IReadOnlyDictionary<string, List<string>> Errors { get; }
}
