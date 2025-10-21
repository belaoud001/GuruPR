namespace GuruPR.Application.Dtos.Jwt;

public record JwtTokenResult
{
    public string Token { get; init; } = string.Empty;

    public DateTime ExpiresAtUtc { get; init; }
}
