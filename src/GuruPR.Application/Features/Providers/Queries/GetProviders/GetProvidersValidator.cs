using FluentValidation;

namespace GuruPR.Application.Features.Providers.Queries.GetProviders;

public class GetProvidersValidator : AbstractValidator<GetProvidersQuery>
{
    public GetProvidersValidator()
    {
        // No specific validation rules for this query at the moment.
    }
}
