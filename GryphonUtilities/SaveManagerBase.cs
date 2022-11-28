using System.Text.Json;

namespace GryphonUtilities;

public abstract class SaveManagerBase
{
    protected SaveManagerBase(string path, TimeManager? timeManager = null)
    {
        Path = path;
        Locker = new object();

        JsonSerializerOptionsProvider optionsProvider = new(timeManager);
        Options = optionsProvider.PascalCaseOptions;
    }

    protected readonly JsonSerializerOptions Options;
    protected readonly string Path;
    protected readonly object Locker;
}