namespace GryphonUtilities.Save;

public interface IReloadable<in TStateData>
{
    void LoadFrom(TStateData? data);
}