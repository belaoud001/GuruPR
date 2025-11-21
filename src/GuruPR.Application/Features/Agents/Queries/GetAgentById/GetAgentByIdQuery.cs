using GuruPR.Application.Features.Agents.Dtos;

using MediatR;

namespace GuruPR.Application.Features.Agents.Queries.GetAgentById;

public record GetAgentByIdQuery(string Id) : IRequest<AgentDto>;