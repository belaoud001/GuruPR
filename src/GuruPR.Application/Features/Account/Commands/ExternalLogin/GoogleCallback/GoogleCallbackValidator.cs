using GuruPR.Application.Common.Interfaces.Application;

namespace GuruPR.Application.Features.Account.Commands.ExternalLogin.GoogleCallback;

public class GoogleCallbackValidator : ExternalLoginValidator<string>
{
    public GoogleCallbackValidator(IUrlValidator urlValidator) : base(urlValidator)
    {
    }
}
