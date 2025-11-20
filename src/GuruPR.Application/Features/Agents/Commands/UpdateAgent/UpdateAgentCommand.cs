using System.Text.Json.Serialization;

using GuruPR.Application.Common.Markers.Interfaces;
using GuruPR.Application.Features.Agents.Dtos;
using GuruPR.Domain.Entities.Agents;
using GuruPR.Domain.Entities.Agents.Configurations;
using GuruPR.Domain.Entities.Agents.Enums;

using MediatR;

namespace GuruPR.Application.Features.Agents.Commands.UpdateAgent;

public record UpdateAgentCommand : IRequest<AgentDto>, IOwnedEntityRequest<Agent>
{
    public string Id { get; set; } = null!;

    [JsonIgnore]
    public string UserId { get; set; } = null!;

    public string Name { get; set; } = null!;
    public string AvatarUrl { get; set; } = null!;
    public string Description { get; set; } = null!;
    public string Instructions { get; set; } = null!;

    // Tools
    public IList<string> Tools { get; set; } = null!;

    // Model Configuration
    public ModelConfiguration ModelConfiguration { get; set; } = null!;

    // Memory Settings
    public MemoryConfiguration MemoryConfiguration { get; set; } = null!;

    // Metadata
    public AgentStatus Status { get; set; } = AgentStatus.Inactive;
}
