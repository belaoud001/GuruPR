using System.Text.Json;
using System.Text.Json.Serialization;

using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace GuruPR.Persistence.Helpers;

public class DictionaryJsonConverter : ValueConverter<Dictionary<string, object>, string>
{
    private static readonly JsonSerializerOptions DefaultOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        WriteIndented = false,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    public DictionaryJsonConverter(JsonSerializerOptions? options = null)
           : base(
               v => JsonSerializer.Serialize(v, options ?? DefaultOptions),
               v => Deserialize(v, options ?? DefaultOptions)
           )
    {
    }

    private static Dictionary<string, object> Deserialize(string? json, JsonSerializerOptions options)
    {
        if (string.IsNullOrWhiteSpace(json))
        {
            return new Dictionary<string, object>();
        }

        try
        {
            return JsonSerializer.Deserialize<Dictionary<string, object>>(json, options) ?? new Dictionary<string, object>();
        }
        catch (JsonException)
        {
            return new Dictionary<string, object>();
        }
    }
}