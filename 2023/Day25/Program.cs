using Utilities;

long Part1(IReadOnlyList<string> lines)
{
    return 0;
}

var lines = new List<string>(File.ReadAllLines("input.txt"));
var part1 = Part1(lines);

Console.WriteLine($"Part 1: {part1}");

Runner.Benchmark(delegate
{
    Part1(lines);
}, "Day 25");