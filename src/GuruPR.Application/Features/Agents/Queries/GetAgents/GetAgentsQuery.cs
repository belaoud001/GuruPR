using GuruPR.Application.Common.Models;
using GuruPR.Application.Features.Agents.Dtos;

using MediatR;

namespace GuruPR.Application.Features.Agents.Queries.GetAgents;

public record GetAgentsQuery(int Page, int PageSize) : IRequest<PaginatedList<AgentDto>>;