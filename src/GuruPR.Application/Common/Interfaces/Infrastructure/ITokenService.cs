using GuruPR.Domain.Entities;

namespace GuruPR.Application.Common.Interfaces.Infrastructure;

public interface ITokenService
{
    Task IssueNewTokenPairAsync(User user);

    Task RenewAccessTokenAsync(User user);

    Task RevokeTokensAsync(User user);
}
