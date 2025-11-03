using GuruPR.Domain.Entities.Configurations;
using GuruPR.Domain.Entities.Configurations.Enums;

namespace GuruPR.Application.Dtos.Agent;

public class UpdateAgentRequest
{
    public string? Name { get; set; }
    public string? AvatarUrl { get; set; }
    public string? Description { get; set; }
    public string? Instrunctions { get; set; }

    // Tools
    public IList<string>? Tools { get; set; }

    // Model Configuration
    public ModelConfiguration? ModelConfiguration { get; set; }

    // Memory Settings
    public MemoryConfiguration? MemoryConfiguration { get; set; }

    // Metadata
    public AgentStatus? Status { get; set; }
}
