using JetBrains.Annotations;

namespace GryphonUtilities.Save;

[PublicAPI]
public interface ICreatableFromState<out TSelf, in TStateData>
    where TSelf : ICreatableFromState<TSelf, TStateData>
{
    static abstract TSelf? Load(TStateData? data);
}