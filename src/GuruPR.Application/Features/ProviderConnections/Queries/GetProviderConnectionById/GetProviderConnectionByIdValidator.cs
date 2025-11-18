using FluentValidation;

using GuruPR.Application.Common.Extensions.Validation;

namespace GuruPR.Application.Features.ProviderConnections.Queries.GetProviderConnectionById;

public class GetProviderConnectionByIdValidator : AbstractValidator<GetProviderConnectionByIdQuery>
{
    public GetProviderConnectionByIdValidator()
    {
        RuleFor(query => query.Id).ValidId("Provider connection");
    }
}
