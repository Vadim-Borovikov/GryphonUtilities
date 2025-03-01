namespace GryphonUtilities.Save;

public interface IFinalData<TSaveData>
    where TSaveData : class
{
    TSaveData? Save();

    void LoadFrom(TSaveData? data);
}