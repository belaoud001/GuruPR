using System.Text.Json.Serialization;

using GuruPR.Application.Common.Markers;
using GuruPR.Domain.Entities.Agents;

using MediatR;

namespace GuruPR.Application.Features.Agents.Commands.DeleteAgent;

public record DeleteAgentCommand(string Id) : IRequest, IOwnedEntityRequest<Agent>
{
    [JsonIgnore]
    public string UserId { get; set; } = null!;
}
