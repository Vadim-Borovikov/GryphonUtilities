using JetBrains.Annotations;

namespace GryphonUtilities.Extensions;

[PublicAPI]
public static class ExceptionExtensions
{
    public static IEnumerable<Exception> FlattenAll(this Exception ex)
    {
        if (ex is AggregateException aggregateException)
        {
            foreach (Exception e in aggregateException.Flatten().InnerExceptions)
            {
                yield return e;
            }
            yield break;
        }

        yield return ex;
        while (ex.InnerException is not null)
        {
            ex = ex.InnerException;
            yield return ex;
        }
    }
}