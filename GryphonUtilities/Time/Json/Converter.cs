using System.Text.Json;
using System.Text.Json.Serialization;
using GryphonUtilities.Extensions;
using JetBrains.Annotations;

namespace GryphonUtilities.Time.Json;

[PublicAPI]
public sealed class Converter : JsonConverter<DateTimeFull>
{
    public Converter(Clock? clock = null) => _clock = clock ?? new Clock();

    public override DateTimeFull Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        string value = reader.GetString().Denull();
        if (DateTimeFull.TryParse(value, out DateTimeFull result))
        {
            return result;
        }
        DateTimeOffset dateTimeOffset = DateTimeOffset.Parse(value);
        return _clock.Denull().GetDateTimeFull(dateTimeOffset);
    }

    public override void Write(Utf8JsonWriter writer, DateTimeFull dateTimeFullValue, JsonSerializerOptions options)
    {
        writer.WriteStringValue(dateTimeFullValue.ToString());
    }

    private readonly Clock? _clock;
}