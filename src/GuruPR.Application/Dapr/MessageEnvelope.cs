using System.Text.Json.Serialization;
using GuruPR.Application.Dapr.Enums;

namespace GuruPR.Application.Dapr;

public class MessageEnvelope
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public OperationType OperationType { get; set; }
    
    public Dictionary<string, object> Metadatas { get; set; }
}
