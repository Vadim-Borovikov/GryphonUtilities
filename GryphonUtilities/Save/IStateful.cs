namespace GryphonUtilities.Save;

public interface IStateful<TStateData>
{
    TStateData? Save();

    void LoadFrom(TStateData? data);
}