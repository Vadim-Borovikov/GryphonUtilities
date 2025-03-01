using JetBrains.Annotations;
using System.Text.Json;
using GryphonUtilities.Time;
using GryphonUtilities.Time.Json;

namespace GryphonUtilities.Save;

[PublicAPI]
public class SaveManager<TData> where TData : class, new()
{
    public TData SaveData { get; protected set; }

    public SaveManager(string path, Clock? clock = null)
    {
        SaveData = new TData();

        _path = path;
        _locker = new object();

        SerializerOptionsProvider optionsProvider = new(clock);
        _options = optionsProvider.PascalCaseOptions;
    }

    public virtual void Load()
    {
        lock (_locker)
        {
            if (!File.Exists(_path))
            {
                return;
            }
            string json = File.ReadAllText(_path);
            SaveData = JsonSerializer.Deserialize<TData>(json, _options) ?? new TData();
        }
    }

    public virtual void Save()
    {
        lock (_locker)
        {
            string json = JsonSerializer.Serialize(SaveData, _options);
            File.WriteAllText(_path, json);
        }
    }

    private readonly JsonSerializerOptions _options;
    private readonly string _path;
    private readonly object _locker;
}