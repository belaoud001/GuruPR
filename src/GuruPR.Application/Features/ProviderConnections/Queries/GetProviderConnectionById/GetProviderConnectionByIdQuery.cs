using GuruPR.Application.Features.ProviderConnections.Dtos;

using MediatR;

namespace GuruPR.Application.Features.ProviderConnections.Queries.GetProviderConnectionById;

public record GetProviderConnectionByIdQuery(string Id) : IRequest<ProviderConnectionDto>;
