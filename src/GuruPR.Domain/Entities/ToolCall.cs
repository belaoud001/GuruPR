namespace GuruPR.Domain.Entities;

public class ToolCall
{
    public string Name { get; set; } = null!;

    public string PluginName { get; set; } = null!;

    public string Arguments { get; set; } = null!;

    public string Output { get; set; } = null!;
}
