using System.Security.Claims;

using GuruPR.Domain.Entities;

namespace GuruPR.Application.Common.Interfaces.Application;

public interface IExternalUserFactory
{
    User Create(ClaimsPrincipal claimsPrincipal, string provider, string email);
}
