using JetBrains.Annotations;
using System.Text.Json;
using GryphonUtilities.Time;
using GryphonUtilities.Time.Json;

namespace GryphonUtilities;

[PublicAPI]
public class SaveManager<T> where T : new()
{
    public T SaveData { get; private set; }

    public SaveManager(string path, Clock? clock = null, Action? afterLoad = null, Action? beforeSave = null)
    {
        _afterLoad = afterLoad;
        _beforeSave = beforeSave;
        SaveData = new T();

        _path = path;
        _locker = new object();

        SerializerOptionsProvider optionsProvider = new(clock);
        _options = optionsProvider.PascalCaseOptions;
    }

    public void Load()
    {
        lock (_locker)
        {
            if (!File.Exists(_path))
            {
                return;
            }
            string json = File.ReadAllText(_path);
            SaveData = JsonSerializer.Deserialize<T>(json, _options) ?? new T();
        }
        _afterLoad?.Invoke();
    }

    public void Save()
    {
        _beforeSave?.Invoke();
        lock (_locker)
        {
            string json = JsonSerializer.Serialize(SaveData, _options);
            File.WriteAllText(_path, json);
        }
    }

    private readonly Action? _afterLoad;
    private readonly Action? _beforeSave;

    private readonly JsonSerializerOptions _options;
    private readonly string _path;
    private readonly object _locker;
}