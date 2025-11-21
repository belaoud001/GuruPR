using System.Security.Claims;

using GuruPR.Domain.Enums;
using GuruPR.Domain.Requests;

namespace GuruPR.Application.Common.Interfaces.Application;

public interface IAccountService
{
    Task RegisterAsync(RegisterRequest registerRequest);

    Task LoginAsync(LoginRequest loginRequest);

    Task RefreshTokenAsync(string userId, string refreshToken);

    Task ConfirmEmailAsync(string userId, string token);

    Task<string> GetEmailConfirmationRedirectUrlAsync(bool success);

    Task LogoutAsync(string userId, string refreshToken);

    Task AssignRoleAsync(string userId, UserRole userRole);

    Task RemoveRoleAsync(string userId, UserRole userRole);

    Task LoginWithExternalProviderAsync(ClaimsPrincipal? claimsPrincipal, string provider);
}
