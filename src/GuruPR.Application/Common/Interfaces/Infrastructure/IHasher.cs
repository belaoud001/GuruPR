namespace GuruPR.Application.Common.Interfaces.Infrastructure;
public interface IHasher
{
    string Hash(string input);

    bool Verify(string? hash, string input);
}
