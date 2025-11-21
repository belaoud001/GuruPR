namespace GuruPR.Application.Common.Settings.Database;

public class DefaultAdminSettings
{
    public static string SectionName => "DefaultAdmin";

    public required string FirstName { get; init; }

    public required string LastName { get; init; }

    public required string Email { get; init; }

    public required string Password { get; init; }
}
