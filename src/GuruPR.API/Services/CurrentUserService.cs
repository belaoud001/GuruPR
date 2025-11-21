using GuruPR.Application.Common.Interfaces.Presentation;
using GuruPR.Infrastructure.Identity.Constants;

namespace GuruPR.Services;

public sealed class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public string? UserId
    {
        get
        {
            var principal = _httpContextAccessor.HttpContext?.User;

            if (principal == null || !principal.Identity?.IsAuthenticated == true)
            {
                return null;
            }

            return principal.FindFirst(JwtClaimTypes.Subject)?.Value;
        }
    }
}
