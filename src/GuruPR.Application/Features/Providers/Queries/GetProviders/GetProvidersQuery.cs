using GuruPR.Application.Features.Providers.Dtos;

using MediatR;

namespace GuruPR.Application.Features.Providers.Queries.GetProviders;

public record GetProvidersQuery : IRequest<List<ProviderDto>>;
