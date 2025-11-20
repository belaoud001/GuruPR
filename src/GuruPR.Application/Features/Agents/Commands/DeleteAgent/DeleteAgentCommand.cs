using System.Text.Json.Serialization;

using GuruPR.Application.Common.Markers.Interfaces;
using GuruPR.Domain.Entities.Agents;

using MediatR;

namespace GuruPR.Application.Features.Agents.Commands.DeleteAgent;

public record DeleteAgentCommand(string Id) : IRequest<Unit>, IOwnedEntityRequest<Agent>
{
    [JsonIgnore]
    public string UserId { get; set; } = null!;
}
