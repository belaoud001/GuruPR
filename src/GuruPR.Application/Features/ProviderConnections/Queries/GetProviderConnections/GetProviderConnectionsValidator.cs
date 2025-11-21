using FluentValidation;

using GuruPR.Application.Common.Extensions.Validation;

namespace GuruPR.Application.Features.ProviderConnections.Queries.GetProviderConnections;

public class GetProviderConnectionsValidator : AbstractValidator<GetProviderConnectionsQuery>
{
    public GetProviderConnectionsValidator()
    {
        RuleFor(query => query.ProviderId).ValidId("Provider");
    }
}
