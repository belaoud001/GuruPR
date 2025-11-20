using Microsoft.AspNetCore.Identity;

namespace GuruPR.Domain.Entities;

public class User : IdentityUser<Guid>
{
    public required string FirstName { get; set; }

    public required string LastName { get; set; }

    public string? RefreshTokenHash { get; set; }

    public DateTime? RefreshTokenExpiryTime { get; set; }

    private string FullName => $"{FirstName} {LastName}".Trim();

    public override string ToString() => FullName;

    public void UpdateRefreshToken(string tokenHash, DateTime expiryTime)
    {
        if (string.IsNullOrWhiteSpace(tokenHash))
        {
            throw new ArgumentException("Token hash cannot be empty.", nameof(tokenHash));
        }

        if (RefreshTokenHash != null && expiryTime <= DateTime.UtcNow)
        {
            throw new ArgumentException("Expiry time must be in the future.", nameof(expiryTime));
        }

        RefreshTokenHash = tokenHash;
        RefreshTokenExpiryTime = expiryTime;
    }

    public void ClearRefreshToken()
    {
        RefreshTokenHash = null;
        RefreshTokenExpiryTime = null;
    }

    public bool NeedsRefreshTokenRenewal(int renewalThresholdDays = 7)
    {
        if (RefreshTokenExpiryTime == null)
        {
            return true;
        }

        var renewalThreshold = DateTime.UtcNow.AddDays(renewalThresholdDays);
        return RefreshTokenExpiryTime < renewalThreshold;
    }

    public bool IsRefreshTokenExpired()
    {
        return RefreshTokenExpiryTime == null || RefreshTokenExpiryTime < DateTime.UtcNow;
    }
}
