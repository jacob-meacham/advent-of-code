namespace Utilities;

public static class EnumerableExtensions
{
    public static IEnumerable<T> Cycle<T>(this IEnumerable<T> sequence)
    {
        while (true)
        {
            foreach (var element in sequence)
            {
                yield return element;
            }
        }
        // ReSharper disable once IteratorNeverReturns
    }
    
    public static IEnumerable<(T previous, T current)> Pairwise<T>(this IEnumerable<T> source)
    {
        using var enumerator = source.GetEnumerator();
        if (!enumerator.MoveNext())
        {
            yield break;
        }

        var previous = enumerator.Current;
        while (enumerator.MoveNext())
        {
            yield return (previous, enumerator.Current);
            previous = enumerator.Current;
        }
    }
    
    public static IEnumerable<TAccumulate> ScanL<TSource, TAccumulate>(
        this IEnumerable<TSource> source, 
        TAccumulate seed, 
        Func<TAccumulate, TSource, TAccumulate> func)
    {
        var accumulated = seed;
        yield return accumulated;
 
        foreach (var item in source)
        {
            accumulated = func(accumulated, item);
            yield return accumulated;
        }
    }
}