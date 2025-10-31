using GuruPR.Domain.Entities.Configurations;
using GuruPR.Domain.Entities.Configurations.Enums;

namespace GuruPR.Domain.Entities;

public class Agent
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Name { get; set; } = null!;
    public string AvatarUrl { get; set; } = null!;
    public string Description { get; set; } = null!;

    // Instructions and its hash for integrity verification
    public string Instrunctions { get; set; } = null!;
    public string InstructionHash { get; set; } = null!;

    // Tools
    public IList<string> Tools { get; set; } = null!;

    // Model Configuration
    public ModelConfiguration ModelConfiguration { get; set; } = null!;

    // Memory Settings
    public MemoryConfiguration MemoryConfiguration { get; set; } = null!;

    // Metadata
    public string CreatedBy { get; set; } = null!;
    public AgentStatus Status { get; set; } = AgentStatus.Inactive;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
