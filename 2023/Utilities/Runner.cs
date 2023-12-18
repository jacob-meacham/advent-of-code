using System.Diagnostics;

namespace Utilities;

public static class Runner
{
    private static string NumSpaces(string label, int total)
    {
        return new string(' ', total-label.Length);
    }
    public static void Benchmark(Action fn, String day, int iterations = 10)
    {
        Stopwatch stopwatch = Stopwatch.StartNew();
        for (var i = 0; i < iterations; i++)
        {
            fn();
        }
        stopwatch.Stop();
        
        var avgMilliseconds = (float)stopwatch.ElapsedMilliseconds / iterations;
        var good = avgMilliseconds < 100 ? "✅" : "❌";
        var timingString = avgMilliseconds.ToString("F1");
        Console.WriteLine($"| {day}{NumSpaces(day, 7)}| {timingString}{NumSpaces(timingString, 12)}|{good}     |");
    }
}