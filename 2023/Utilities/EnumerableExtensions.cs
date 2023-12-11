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
    }
}