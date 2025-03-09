using JetBrains.Annotations;
using GryphonUtilities.Time;

namespace GryphonUtilities.Save;

[PublicAPI]
public class SaveManager<TFinalData, TData> : SaveManager<TData>
    where TFinalData : IFinalData<TData>
    where TData : new()
{
    public TFinalData FinalData { get; private set; }

    public SaveManager(string path, TFinalData finalData, Clock? clock = null)
        : base(path, clock)
    {
        FinalData = finalData;
    }

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