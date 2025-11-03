using GuruPR.Domain.Entities.Configurations;
using GuruPR.Domain.Entities.Configurations.Enums;

namespace GuruPR.Application.Dtos.Agent;

public class AgentDto
{
    public string Id { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string AvatarUrl { get; set; } = null!;
    public string Description { get; set; } = null!;

    public string Instrunctions { get; set; } = null!;

    public IList<string> Tools { get; set; } = null!;

    // Model Configuration
    public ModelConfiguration ModelConfiguration { get; set; } = null!;

    // Memory Settings
    public MemoryConfiguration MemoryConfiguration { get; set; } = null!;

    // Metadata
    public string CreatedBy { get; set; } = null!;
    public AgentStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
}
