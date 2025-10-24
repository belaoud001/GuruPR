namespace GuruPR.Application.Interfaces.Infrastructure;
public interface IHasher
{
    string Hash(string input);

    bool Verify(string? hash, string input);
}
