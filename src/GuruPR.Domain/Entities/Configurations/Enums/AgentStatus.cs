using System.Text.Json.Serialization;

namespace GuruPR.Domain.Entities.Configurations.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum AgentStatus
{
    Active,
    Inactive
}
