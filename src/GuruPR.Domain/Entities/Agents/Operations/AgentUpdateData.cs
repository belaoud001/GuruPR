using GuruPR.Domain.Entities.Agents.Configurations;
using GuruPR.Domain.Entities.Agents.Enums;

namespace GuruPR.Domain.Entities.Agents.Operations;

public class AgentUpdateData
{
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
