using Microsoft.AspNetCore.Mvc;

namespace GuruPR.Application.Common.Interfaces.Infrastructure;

public interface IExternalAuthService
{
    Task<ChallengeResult> InitiateGoogleLoginAsync(string? returnUrl);

    Task<string> HandleGoogleCallbackAsync(string returnUrl);
}
