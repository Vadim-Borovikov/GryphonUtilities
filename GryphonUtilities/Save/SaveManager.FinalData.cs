using JetBrains.Annotations;
using GryphonUtilities.Time;

namespace GryphonUtilities.Save;

[PublicAPI]
public class SaveManager<TFinalData, TData> : SaveManager<TData>
    where TFinalData : class, IFinalData<TData>, new()
    where TData : class, new()
{
    public TFinalData FinalData { get; private set; }

    public SaveManager(string path, Clock? clock = null) : base(path, clock) => FinalData = new TFinalData();

    public override void Load()
    {
        base.Load();
        FinalData.LoadFrom(SaveData);
    }

    public override void Save()
    {
        SaveData = FinalData.Save() ?? new TData();
        base.Save();
    }
}