using System.Security.Claims;

namespace GuruPR.Application.Common.Interfaces.Application;

public interface IExternalUserProvisioningService
{
    Task LoginWithExternalProviderAsync(ClaimsPrincipal? claimsPrincipal, string provider);
}
