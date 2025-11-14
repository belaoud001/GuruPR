using GuruPR.Application.Settings.Security;
using GuruPR.Domain.Entities;

using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace GuruPR.Persistence.Identity.IdentityValidators;

public class StrictEmailDomainValidator : IUserValidator<User>
{
    private readonly HashSet<string> _allowedDomains;

    public StrictEmailDomainValidator(IOptions<EmailValidationSettings> emailValidationSettings)
    {
        _allowedDomains = emailValidationSettings.Value.AllowedDomains;
    }

    #region Public Methods

    public Task<IdentityResult> ValidateAsync(UserManager<User> userManager, User user)
    {
        var errors = new List<IdentityError>();

        if (string.IsNullOrEmpty(user.Email))
        {
            // This case is already handled by the built-in email validator by .NET Identity
            return Task.FromResult(IdentityResult.Failed());
        }

        var emailParts = user.Email.Split('@');
        if (emailParts.Length != 2)
        {
            // TODO: Add regional variant support
            errors.Add(new IdentityError
            {
                Code = "EmailInvalidFormat",
                Description = "Email format is invalid."
            }
            );
        }
        else
        {
            var domain = emailParts[1].ToLowerInvariant().Trim();
            if (string.IsNullOrWhiteSpace(domain))
            {
                errors.Add(new IdentityError
                {
                    Code = "EmailMissingDomain",
                    Description = "Email domain is missing."
                }
                );
            }
            else if (!_allowedDomains.Contains(domain))
            {
                errors.Add(new IdentityError
                {
                    Code = "EmailInvalidDomain",
                    Description = $"Email domain '@{domain}' is not accepted. Please use a supported email provider."
                }
                );
            }
        }

        return Task.FromResult(errors.Any() ? IdentityResult.Failed(errors.ToArray())
                                            : IdentityResult.Success);
    }

    #endregion
}
