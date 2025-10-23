using GuruPR.Domain.Enums;
using GuruPR.Domain.Requests;

namespace GuruPR.Application.Interfaces.Application;

public interface IAccountService
{
    Task RegisterAsync(RegisterRequest registerRequest);

    Task LoginAsync(LoginRequest loginRequest);

    Task RefreshTokenAsync(string refreshToken);

    Task ConfirmEmailAsync(string userId, string token);

    Task LogoutAsync(string userId, string refreshToken);

    Task AssignRoleAsync(string userId, UserRole userRole);

    Task RemoveRoleAsync(string userId, UserRole userRole);

}
