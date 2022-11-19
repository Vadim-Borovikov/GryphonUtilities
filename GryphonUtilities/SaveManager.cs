using JetBrains.Annotations;
using System.Text.Json;

namespace GryphonUtilities;

[PublicAPI]
public class SaveManager<T> : SaveManagerBase
    where T: new()
{
    public T Data { get; private set; }

    public SaveManager(string path, TimeManager? timeManager = null) : base(path, timeManager) => Data = new T();

    public void Save()
    {
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
    }
}