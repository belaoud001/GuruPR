using System.Text.Json.Serialization;

namespace GuruPR.Domain.Entities.Agents.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum AgentStatus
{
    Active,
    Inactive
}
