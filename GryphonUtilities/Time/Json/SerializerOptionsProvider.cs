using System.Text.Encodings.Web;
using System.Text.Json;
using JetBrains.Annotations;
using JoyMoe.Common.Json;

namespace GryphonUtilities.Time.Json;

[PublicAPI]
public class SerializerOptionsProvider
{
    public readonly JsonSerializerOptions PascalCaseOptions;
    public readonly JsonSerializerOptions CamelCaseOptions;
    public readonly JsonSerializerOptions SnakeCaseOptions;

    public SerializerOptionsProvider(Clock? clock = null)
    {
        Converter converter = new(clock);
        PascalCaseOptions = CreateOptionsWith(converter);

        CamelCaseOptions = CreateOptionsWith(converter);
        CamelCaseOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;

        SnakeCaseOptions = CreateOptionsWith(converter);
        SnakeCaseOptions.PropertyNamingPolicy = new SnakeCaseNamingPolicy();
    }
    private static JsonSerializerOptions CreateOptionsWith(Converter converter)
    {
        JsonSerializerOptions options = new()
        {
            WriteIndented = true,
            IncludeFields = true,
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            PropertyNameCaseInsensitive = true,
        };
        options.Converters.Add(converter);
        return options;
    }
}