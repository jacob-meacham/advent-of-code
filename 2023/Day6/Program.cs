﻿using System.Text.RegularExpressions;
using Utilities;

long SolveRace(long time, long distance)
{
    // Quadratic equation of the form (time-x)(x) = distance
    // -x2+time-distance = 0
    var a = -1L;
    var b = time;
    var c = -distance;
        
    // Assuming well-formed input
    double discriminant = b * b - 4 * a * c;
    double sqrtDiscriminant = Math.Sqrt(discriminant);
    var x1 = (long)Math.Ceiling((-b + sqrtDiscriminant) / (2 * a));
    var x2 = (long)Math.Floor((-b - sqrtDiscriminant) / (2 * a));

    // Need to best the time
    if ((time - x1) * x1 <= distance)
    {
        x1 += 1;
    }
        
    if ((time - x2) * x2 <= distance)
    {
        x2 -= 1;
    }

    return x2 - x1 + 1L;
}

long Part1(List<string> lines)
{
    var races = LineRegex().Matches(lines[0])
        .Select(m => long.Parse(m.Value))
        .Zip(LineRegex().Matches(lines[1]).Select(m => long.Parse(m.Value))).ToList();
    
    return races
        .Select(race => SolveRace(race.First, race.Second))
        .Aggregate(1L, (acc, n) => acc * n);
}

long Part2(List<string> lines)
{
    long time = long.Parse(string.Join("", lines[0].Split(": ")[1].Split(" ")));
    long distance = long.Parse(string.Join("", lines[1].Split(": ")[1].Split(" ")));
    return SolveRace(time, distance);
}

var lines = new List<string>(File.ReadAllLines("input.txt"));
var part1 = Part1(lines);
var part2 = Part2(lines);

Console.WriteLine($"Part 1: {part1}, Part 2: {part2}");

Runner.Benchmark(delegate
{
    Part1(lines);
    Part2(lines);
}, "Day 6");

partial class Program
{
    [GeneratedRegex(@"\d+")]
    private static partial Regex LineRegex();
}