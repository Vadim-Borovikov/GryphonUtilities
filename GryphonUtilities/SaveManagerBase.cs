using System.Text.Encodings.Web;
using System.Text.Json;

namespace GryphonUtilities;

public abstract class SaveManagerBase
{
    protected SaveManagerBase(string path, TimeManager? timeManager = null)
    {
        Path = path;
        Locker = new object();
        Options.Converters.Add(new DateTimeFullJsonConverter(timeManager));
    }

    protected readonly JsonSerializerOptions Options = new()
    {
        WriteIndented = true,
        IncludeFields = true,
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
    };
    protected readonly string Path;
    protected readonly object Locker;
}