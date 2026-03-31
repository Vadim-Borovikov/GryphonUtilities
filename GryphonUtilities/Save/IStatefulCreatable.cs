using JetBrains.Annotations;

namespace GryphonUtilities.Save;

[PublicAPI]
public interface IStatefulCreatable<out TSelf, TStateData>
    : ISaveable<TStateData>, ICreatableFromState<TSelf, TStateData>
where TSelf : ICreatableFromState<TSelf, TStateData> { }