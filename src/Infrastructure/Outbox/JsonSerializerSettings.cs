using System.Text.Json;
using System.Text.Json.Serialization;

namespace Infrastructure.Outbox;

public static class JsonSerializerSettings
{
    public static readonly JsonSerializerOptions Options = new()
    {
        Converters =
        {
            new JsonStringEnumConverter()
        }
    };
}