using System.Text.Json;
using System.Text.Json.Serialization;

namespace GryphonUtilities;

internal sealed class DateTimeFullJsonConverter : JsonConverter<DateTimeFull>
{
    public DateTimeFullJsonConverter(TimeManager? timeManager = null) => _timeManager = timeManager;

    public override DateTimeFull Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        string input = reader.GetString() ?? "";
        DateTimeFull? result =
            _timeManager is null ? DateTimeFull.Parse(input) : _timeManager.ParseDateTimeFull(input);
        return result.GetValue();
    }

    public override void Write(Utf8JsonWriter writer, DateTimeFull dateTimeFullValue, JsonSerializerOptions options)
    {
        writer.WriteStringValue(dateTimeFullValue.ToString());
    }

    private readonly TimeManager? _timeManager;
}