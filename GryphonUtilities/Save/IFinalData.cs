namespace GryphonUtilities.Save;

public interface IFinalData<out TFinalData, TSaveData>
    where TFinalData : class, IFinalData<TFinalData, TSaveData>
    where TSaveData : class
{
    TSaveData? Save();

    static abstract TFinalData? Load(TSaveData data);
}