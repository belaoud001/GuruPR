namespace GuruPR.Domain.Requests;

public class LogoutRequest
{
    public required string UserId { get; init; }

    public required string RefreshToken { get; init; }
}
