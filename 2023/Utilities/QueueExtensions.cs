namespace Utilities;

public static class QueueExtensions
{
    public static void EnqueueRange<T>(this Queue<T> queue, IEnumerable<T> source)
    {
        foreach (var item in source)
        {
            queue.Enqueue(item);
        }
    }
}