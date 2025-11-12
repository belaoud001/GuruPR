using GuruPR.Domain.Entities.Agents.Configurations;
using GuruPR.Domain.Entities.Agents.Enums;

namespace GuruPR.Application.Features.Agents.Dtos;

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
    public string UserId { get; set; } = null!;
    public AgentStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
}
