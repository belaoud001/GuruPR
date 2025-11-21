using GuruPR.Application.Features.Providers.Dtos;

using MediatR;

namespace GuruPR.Application.Features.Providers.Queries.GetProviderById;

public record GetProviderByIdQuery(string ProviderId) : IRequest<ProviderDto>;
