using System.Security.Cryptography;
using System.Text;

using GuruPR.Application.Common.Interfaces.Infrastructure;
using GuruPR.Application.Settings.Security;

using Microsoft.Extensions.Options;

namespace GuruPR.Infrastructure.Services.Security;

public class TokenEncryptionService : ITokenEncryptionService
{
    private readonly TokenEncryptionSettings _tokenEncryptionSettings;
    private readonly byte[] _key;
    private const int IvSize = 16;

    public TokenEncryptionService(IOptions<TokenEncryptionSettings> tokenEncryptionSettings)
    {
        _tokenEncryptionSettings = tokenEncryptionSettings.Value;

        var key = _tokenEncryptionSettings.Key;

        if (string.IsNullOrEmpty(key) || key.Length != 32)
        {
            throw new ArgumentException("Encryption key cannot be null or empty", nameof(key));
        }

        using var sha256 = SHA256.Create();
        _key = sha256.ComputeHash(Encoding.UTF8.GetBytes(key));
    }

    public string Encrypt(string plainText)
    {
        if (string.IsNullOrEmpty(plainText))
        {
            return plainText;
        }

        using var aes = Aes.Create();
        aes.Key = _key;
        aes.GenerateIV();

        using var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
        using var memoryStream = new MemoryStream();

        memoryStream.Write(aes.IV, 0, aes.IV.Length);

        using (var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
        using (var streamWriter = new StreamWriter(cryptoStream))
        {
            streamWriter.Write(plainText);
        }

        return Convert.ToBase64String(memoryStream.ToArray());
    }

    public string Decrypt(string cipherText)
    {
        if (string.IsNullOrEmpty(cipherText))
        {
            return cipherText;
        }

        try
        {
            var fullCipher = Convert.FromBase64String(cipherText);

            using var aes = Aes.Create();
            aes.Key = _key;

            var iv = new byte[IvSize];
            Array.Copy(fullCipher, 0, iv, 0, iv.Length);
            aes.IV = iv;

            using var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
            using var memoryStream = new MemoryStream(fullCipher, iv.Length, fullCipher.Length - iv.Length);
            using var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read);
            using var streamReader = new StreamReader(cryptoStream);

            return streamReader.ReadToEnd();
        }
        catch (Exception exception)
        {
            throw new CryptographicException("Failed to decrypt token", exception);
        }
    }
}
