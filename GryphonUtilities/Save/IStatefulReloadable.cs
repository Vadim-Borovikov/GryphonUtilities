namespace GryphonUtilities.Save;

public interface IStatefulReloadable<TStateData> : ISaveable<TStateData>, IReloadable<TStateData> { }