namespace GuruPR.Domain.Entities;

public class Tool
{
    public string Id { get; set; } = Guid.NewGuid().ToString();

    public string Name { get; set; } = null!;

    public string Type { get; set; } = null!;

    public string Description { get; set; } = null!;

    public Dictionary<string, object> ParametersSchema { get; set; } = null!;

    public IDictionary<string, object> Config { get; set; } = null!;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
