using GuruPR.Domain.Entities.Agents.Configurations;
using GuruPR.Domain.Entities.Agents.Enums;
using GuruPR.Domain.Entities.Agents.Operations;
using GuruPR.Domain.Interfaces.Markers;

namespace GuruPR.Domain.Entities.Agents;

public class Agent : IOwnedEntity
{
    // Basic Info
    public string Id { get; set; } = Guid.NewGuid().ToString();
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
    public string UserId { get; set; } = null!;
    public AgentStatus Status { get; set; } = AgentStatus.Inactive;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public bool IsActive() => Status == AgentStatus.Active;

    public void Update(AgentUpdateData agentUpdateData)
    {
        Name = agentUpdateData.Name;
        AvatarUrl = agentUpdateData.AvatarUrl;
        Description = agentUpdateData.Description;
        Instructions = agentUpdateData.Instructions;

        Tools = agentUpdateData.Tools;

        ModelConfiguration = agentUpdateData.ModelConfiguration;
        MemoryConfiguration = agentUpdateData.MemoryConfiguration;

        Status = agentUpdateData.Status;
    }
}
