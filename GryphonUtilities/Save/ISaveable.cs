namespace GryphonUtilities.Save;

public interface ISaveable<out TStateData>
{
    TStateData? Save();
}