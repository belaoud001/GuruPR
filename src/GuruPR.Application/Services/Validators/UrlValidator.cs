using GuruPR.Application.Exceptions.Account;
using GuruPR.Application.Interfaces.Application;
using GuruPR.Application.Settings.Security;

using Microsoft.Extensions.Options;

namespace GuruPR.Application.Services.Validators;

public class UrlValidator : IUrlValidator
{
    private readonly AllowedOriginsSettings _allowedOriginsSettings;

    public UrlValidator(IOptions<AllowedOriginsSettings> allowedOriginsSettings)
    {
        _allowedOriginsSettings = allowedOriginsSettings.Value;
    }

    public void ValidateReturnUrl(string? returnUrl)
    {
        if (string.IsNullOrWhiteSpace(returnUrl))
        {
            throw new InvalidReturnUrlException("Return URL cannot be empty.");
        }

        if (!Uri.TryCreate(returnUrl, UriKind.Absolute, out var uri))
        {
            throw new InvalidReturnUrlException("Return URL is not a valid absolute URI.");
        }

        if (_allowedOriginsSettings.Origins == null || _allowedOriginsSettings.Origins.Count == 0)
        {
            throw new MissingAllowedOriginsException("No allowed origins are configured.");
        }

        var returnOrigin = $"{uri.Scheme}://{uri.Authority}";
        var isAllowed = _allowedOriginsSettings.Origins.Any(
            allowed => allowed.TrimEnd('/')
                              .Equals(returnOrigin, StringComparison.OrdinalIgnoreCase));

        if (!isAllowed)
        {
            throw new UntrustedReturnUrlException(returnUrl);
        }
    }
}
