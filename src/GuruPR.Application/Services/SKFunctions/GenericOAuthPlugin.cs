namespace GuruPR.Application.Services.SKFunctions;

public class GenericOAuthPlugin
{
    private readonly HttpClient _httpClient;

    public GenericOAuthPlugin(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }
}
