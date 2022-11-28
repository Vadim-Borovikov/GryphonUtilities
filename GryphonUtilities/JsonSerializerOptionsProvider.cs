using System.Text.Encodings.Web;
using System.Text.Json;
using JetBrains.Annotations;
using JoyMoe.Common.Json;

namespace GryphonUtilities;

[PublicAPI]
public class JsonSerializerOptionsProvider
{
    public readonly JsonSerializerOptions PascalCaseOptions;
    public readonly JsonSerializerOptions CamelCaseOptions;
    public readonly JsonSerializerOptions SnakeCaseOptions;

    public JsonSerializerOptionsProvider(TimeManager? timeManager = null)
    {
        DateTimeFullJsonConverter converter = new(timeManager);
        PascalCaseOptions = CreateOptionsWith(converter);

        CamelCaseOptions = CreateOptionsWith(converter);
        CamelCaseOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;

        SnakeCaseOptions = CreateOptionsWith(converter);
        SnakeCaseOptions.PropertyNamingPolicy = new SnakeCaseNamingPolicy();
    }
    private static JsonSerializerOptions CreateOptionsWith(DateTimeFullJsonConverter converter)
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