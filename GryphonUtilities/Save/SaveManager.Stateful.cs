using JetBrains.Annotations;
using GryphonUtilities.Time;

namespace GryphonUtilities.Save;

[PublicAPI]
public class SaveManager<TState, TStateData>
    where TState : IStateful<TStateData>
    where TStateData : new()
{
    public SaveManager(string path, Clock? clock = null)
    {
        _internal = new SaveManager<TStateData>(path, clock);
    }

    public void LoadTo(TState target)
    {
        _internal.Load();
        target.LoadFrom(_internal.SaveData);
    }

    public void Save(TState source)
    {
        _internal.SaveData = source.Save() ?? new TStateData();
        _internal.Save();
    }

    private readonly SaveManager<TStateData> _internal;
}