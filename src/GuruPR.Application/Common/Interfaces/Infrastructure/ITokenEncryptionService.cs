namespace GuruPR.Application.Common.Interfaces.Infrastructure;

public interface ITokenEncryptionService
{
    /// <summary>
    /// Encrypts the given plain text into a cipher text.
    /// </summary>
    /// <param name="plainText">The plain text to encrypt.</param>
    /// <returns>The encrypted cipher text.</returns>
    string Encrypt(string plainText);

    /// <summary>
    /// Decrypts the given cipher text back into plain text.
    /// </summary>
    /// <param name="cipherText">The cipher text to decrypt.</param>
    /// <returns>The decrypted plain text.</returns>
    string Decrypt(string cipherText);
}
