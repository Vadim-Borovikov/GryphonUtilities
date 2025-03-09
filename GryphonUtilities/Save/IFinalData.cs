namespace GryphonUtilities.Save;

public interface IFinalData<TSaveData>
{
    TSaveData? Save();

    void LoadFrom(TSaveData? data);
}