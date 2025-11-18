using GuruPR.Application.Features.ProviderConnections.Dtos;

using MediatR;

namespace GuruPR.Application.Features.ProviderConnections.Queries.GetProviderConnections;

public record GetProviderConnectionsQuery(string ProviderId) : IRequest<List<ProviderConnectionDto>>;
