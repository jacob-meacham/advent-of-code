using System.Diagnostics;

namespace Utilities;

public class Runner
{
    // TODO: Return the result?
    // public delegate T ProfileFunction<T>();
    public static void Benchmark(Action fn, String day, int iterations = 10)
    {
        Stopwatch stopwatch = Stopwatch.StartNew();
        for (int i = 0; i < iterations; i++)
        {
            fn();
        }
        stopwatch.Stop();
        
        float avgMilliseconds = (float)stopwatch.ElapsedMilliseconds / iterations;
        String good = avgMilliseconds < 250 ? "✅" : "❌";
        Console.WriteLine($"| {day}  | {avgMilliseconds}        |{good}      |");
    }
}