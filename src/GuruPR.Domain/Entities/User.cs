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
}
