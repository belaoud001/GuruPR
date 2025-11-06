using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GuruPR.Application.Interfaces.Infrastructure;

public interface IExternalAuthService
{
    Task<ChallengeResult> InitiateGoogleLoginAsync(string? returnUrl, HttpContext httpContext);

    Task<string> HandleGoogleCallbackAsync(string returnUrl, HttpContext httpContext);
}
