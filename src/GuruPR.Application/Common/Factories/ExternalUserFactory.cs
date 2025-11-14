using System.Security.Claims;

using GuruPR.Application.Common.Interfaces.Application;
using GuruPR.Domain.Entities;

namespace GuruPR.Application.Common.Factories;

public class ExternalUserFactory : IExternalUserFactory
{
    public User Create(ClaimsPrincipal principal, string provider, string email)
    {
        return new User
        {
            Email = email,
            UserName = email,
            FirstName = principal.FindFirstValue(ClaimTypes.GivenName) ?? provider.Normalize(),
            LastName = principal.FindFirstValue(ClaimTypes.Surname) ?? "User",
            EmailConfirmed = true
        };
    }
}
