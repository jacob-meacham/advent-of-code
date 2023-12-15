using System.Diagnostics;

namespace Utilities;

public static class Runner
{
    public static void Benchmark(Action fn, String day, int iterations = 10)
    {
        Stopwatch stopwatch = Stopwatch.StartNew();
        for (var i = 0; i < iterations; i++)
        {
            fn();
        }
        stopwatch.Stop();
        
        var avgMilliseconds = (float)stopwatch.ElapsedMilliseconds / iterations;
        var good = avgMilliseconds < 250 ? "✅" : "❌";
        Console.WriteLine($"| {day}  | {avgMilliseconds}        |{good}      |");
    }
}