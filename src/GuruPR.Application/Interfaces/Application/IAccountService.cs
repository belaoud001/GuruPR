using GuruPR.Domain.Requests;

namespace GuruPR.Application.Interfaces.Application;

public interface IAccountService
{
    Task RegisterAsync(RegisterRequest registerRequest);

    Task LoginAsync(LoginRequest loginRequest);

    Task RefreshTokenAsync(string refreshToken);

}
