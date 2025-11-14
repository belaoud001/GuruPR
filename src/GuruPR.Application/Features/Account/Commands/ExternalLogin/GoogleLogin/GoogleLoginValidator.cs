using GuruPR.Application.Common.Interfaces.Application;

using Microsoft.AspNetCore.Mvc;

namespace GuruPR.Application.Features.Account.Commands.ExternalLogin.GoogleLogin;

public class GoogleLoginValidator : ExternalLoginValidator<ChallengeResult>
{
    public GoogleLoginValidator(IUrlValidator urlValidator) : base(urlValidator)
    {
    }
}
