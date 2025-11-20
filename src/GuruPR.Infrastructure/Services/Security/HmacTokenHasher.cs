using System.Security.Cryptography;
using System.Text;

using GuruPR.Application.Common.Interfaces.Infrastructure;
using GuruPR.Application.Common.Settings.Security;

using Microsoft.Extensions.Options;

namespace GuruPR.Infrastructure.Services.Security;

public class HmacTokenHasher : IHasher
{
    private TokenHashingSettings _tokenHashingSettings;
    private readonly byte[] _secretKey;

    public HmacTokenHasher(IOptions<TokenHashingSettings> tokenHashingSettings)
    {
        _tokenHashingSettings = tokenHashingSettings.Value;

        var secretKey = _tokenHashingSettings.Key;
        if (string.IsNullOrEmpty(secretKey))
        {
            throw new ArgumentException("Secret key cannot be null or empty", nameof(secretKey));
        }

        _secretKey = Encoding.UTF8.GetBytes(secretKey);
    }

    public string Hash(string token)
    {
        if (string.IsNullOrEmpty(token))
        {
            throw new ArgumentException("Token cannot be null or empty", nameof(token));
        }

        using var hmac = new HMACSHA256(_secretKey);
        byte[] hashBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(token));

        return Convert.ToBase64String(hashBytes);
    }

    public bool Verify(string? hashedToken, string token)
    {
        if (string.IsNullOrEmpty(hashedToken) || string.IsNullOrEmpty(token))
        {
            return false;
        }

        using var hmac = new HMACSHA256(_secretKey);
        byte[] computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(token));

        try
        {
            byte[] storedHash = Convert.FromBase64String(hashedToken);

            return CryptographicOperations.FixedTimeEquals(storedHash, computedHash);
        }
        catch (FormatException)
        {
            return false;
        }
    }
}
