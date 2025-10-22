using GuruPR.Domain.Enums;

namespace GuruPR.Domain.Extensions.User;

public static class UserRoleExtensions
{
    public static string ToName(this UserRole role) => role.ToString();

    public static IEnumerable<string> AllRoleNames() => Enum.GetNames(typeof(UserRole));
}
