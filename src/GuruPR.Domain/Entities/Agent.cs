namespace GuruPR.Domain.Entities;

public class Agent
{
    public string Id { get; set; } = Guid.NewGuid().ToString();

    public string Name { get; set; } = null!;

    public string AvatarUrl { get; set; } = null!;

    public string Description { get; set; } = null!;

    public string InstructionHash { get; set; } = null!;

    public IList<string> Tools { get; set; } = null!;

    public IDictionary<string, object> ModelConfig { get; set; } = null!;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
