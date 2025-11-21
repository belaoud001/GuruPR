using GuruPR.Application.Common.Interfaces.Application;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace GuruPR.Application.Features.Account.Services;

public class AccountLinkGenerator : IAccountLinkGenerator
{
    private readonly LinkGenerator _linkGenerator;
    private readonly IHttpContextAccessor _httpContextAccessor;

    private const string ConfirmEmailEndpointName = "ConfirmEmail";

    public AccountLinkGenerator(LinkGenerator linkGenerator, IHttpContextAccessor httpContextAccessor)
    {
        _linkGenerator = linkGenerator;
        _httpContextAccessor = httpContextAccessor;
    }

    public string GenerateConfirmationLink(Guid userId, string token)
    {
        return GenerateLink(ConfirmEmailEndpointName, new { userId, token });
    }

    private string GenerateLink(string endpointName, object values)
    {
        var httpContext = _httpContextAccessor.HttpContext
                          ?? throw new InvalidOperationException("HTTP context is not available.");

        var link = _linkGenerator.GetUriByName(httpContext,
                                               endpointName: endpointName,
                                               values: values);

        return link ?? throw new InvalidOperationException($"Failed to generate link for action '{endpointName}'.");
    }
}
