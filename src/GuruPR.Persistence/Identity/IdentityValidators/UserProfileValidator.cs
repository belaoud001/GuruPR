using System.Text.RegularExpressions;

using GuruPR.Domain.Entities;

using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace GuruPR.Persistence.Identity.IdentityValidators;

public class UserProfileValidator : IUserValidator<User>
{
    // Regex to validate usernames (first or last):
    // - Allows Unicode letters (\p{L})
    // - Allows spaces, apostrophes ('), hyphens (-), and dots (.) **only between letters**
    // - Prevents names from starting or ending with non-letter characters
    // - Supports single-letter names
    // - Compiled for performance and culture-invariant for consistent behavior across regions
    private static readonly Regex NameRegex = new Regex(@"^[\p{L}](?:[\p{L}]|[ '\-\.](?=[\p{L}]))*[\p{L}]$|^[\p{L}]$",
                                                        RegexOptions.CultureInvariant | RegexOptions.Compiled | RegexOptions.Singleline,
                                                        TimeSpan.FromMilliseconds(200));

    private readonly ILogger<UserProfileValidator> _logger;

    public UserProfileValidator(ILogger<UserProfileValidator> logger)
    {
        _logger = logger;
    }

    #region Public Methods

    public Task<IdentityResult> ValidateAsync(UserManager<User> manager, User user)
    {
        var errors = new List<IdentityError>();

        // Validate FirstName
        if (string.IsNullOrWhiteSpace(user.FirstName))
        {
            errors.Add(new IdentityError
            {
                Code = "FirstNameRequired",
                Description = "First name is required."
            });
        }
        else
        {
            IsValidName(user.FirstName, "FirstName", errors);
        }

        // Validate LastName
        if (string.IsNullOrWhiteSpace(user.LastName))
        {
            errors.Add(new IdentityError
            {
                Code = "LastNameRequired",
                Description = "Last name is required."
            });
        }
        else
        {
            IsValidName(user.LastName, "LastName", errors);
        }

        return Task.FromResult(errors.Any() ? IdentityResult.Failed(errors.ToArray())
                                            : IdentityResult.Success);
    }

    #endregion

    #region Private Methods

    private void IsValidName(string name, string fieldName, List<IdentityError> errors)
    {
        if (name.Length < 2)
        {
            errors.Add(new IdentityError
            {
                Code = $"{fieldName}TooShort",
                Description = $"{fieldName} must be at least 2 characters long."
            });

            return;
        }

        if (name.Length > 50)
        {
            errors.Add(new IdentityError
            {
                Code = $"{fieldName}TooLong",
                Description = $"{fieldName} must not exceed 50 characters."
            });

            return;
        }

        try
        {
            if (!NameRegex.IsMatch(name))
            {
                errors.Add(new IdentityError
                {
                    Code = $"{fieldName}Invalid",
                    Description = $"{fieldName.Replace("Name", " name")} contains invalid characters."
                });
                return;
            }
        }
        catch (RegexMatchTimeoutException)
        {
            _logger.LogWarning("Regex timeout during {FieldName} validation. Value length: {Length}. Possible attack attempt.", fieldName, name.Length);

            errors.Add(new IdentityError
            {
                Code = $"{fieldName}ValidationFailed",
                Description = $"{fieldName} validation failed. Please try again or contact support."
            });

            return;
        }

        return;
    }

    #endregion
}
