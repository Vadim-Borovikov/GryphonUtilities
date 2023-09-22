using JetBrains.Annotations;
using System.Text.Json;

namespace GryphonUtilities;

[PublicAPI]
public class SaveManager<T> : SaveManagerBase
    where T: new()
{
    public T Data { get; private set; }

    public SaveManager(string path, TimeManager? timeManager = null, Action? afterLoad = null,
        Action? beforeSave = null) : base(path, timeManager)
    {
        _afterLoad = afterLoad;
        _beforeSave = beforeSave;
        Data = new T();
    }

    public void Save()
    {
        _beforeSave?.Invoke();
        lock (Locker)
        {
            string json = JsonSerializer.Serialize(Data, Options);
            File.WriteAllText(Path, json);
        }
    }

    public void Load()
    {
        lock (Locker)
        {
            if (!File.Exists(Path))
            {
                return;
            }
            string json = File.ReadAllText(Path);
            Data = JsonSerializer.Deserialize<T>(json, Options) ?? new T();
        }
        _afterLoad?.Invoke();
    }

    private readonly Action? _afterLoad;
    private readonly Action? _beforeSave;
}