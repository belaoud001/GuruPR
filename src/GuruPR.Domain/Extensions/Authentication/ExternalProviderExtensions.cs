namespace GuruPR.Domain.Extensions.Authentication;

public static class ExternalProviderExtensions
{
    public static string ToName(this Enums.ExternalProvider provider) => provider.ToString();

    public static IEnumerable<string> AllProviderNames() => Enum.GetNames(typeof(Enums.ExternalProvider));
}
