using System.Text.Json;
using System.Text.Json.Serialization;
using GryphonUtilities.Extensions;
using JetBrains.Annotations;

namespace GryphonUtilities;

[PublicAPI]
public sealed class DateTimeFullJsonConverter : JsonConverter<DateTimeFull>
{
    public DateTimeFullJsonConverter(TimeManager? timeManager = null) => _timeManager = timeManager;

    public override DateTimeFull Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        string value = reader.GetString().Denull();
        if (DateTimeFull.TryParse(value, out DateTimeFull result))
        {
            return result;
        }
        DateTimeOffset dateTimeOffset = DateTimeOffset.Parse(value);
        return _timeManager.Denull().GetDateTimeFull(dateTimeOffset);
    }

    public override void Write(Utf8JsonWriter writer, DateTimeFull dateTimeFullValue, JsonSerializerOptions options)
    {
        writer.WriteStringValue(dateTimeFullValue.ToString());
    }

    private readonly TimeManager? _timeManager;
}