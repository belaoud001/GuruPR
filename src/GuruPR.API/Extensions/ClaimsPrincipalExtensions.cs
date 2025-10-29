using System.Security.Claims;

namespace GuruPR.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static string? GetClaimValue(this ClaimsPrincipal user, string claimType)
    {
        return user?.Claims.FirstOrDefault(claim => claim.Type == claimType)?.Value;
    }
}
