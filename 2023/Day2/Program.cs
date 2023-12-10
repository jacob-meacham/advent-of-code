using System.Text.RegularExpressions;
using Utilities;

bool IsDrawPossible(Draw draw, Draw maxDraw)
{
    if (draw.Red > maxDraw.Red)
    {
        return false;
    }

    if (draw.Green > maxDraw.Green)
    {
        return false;
    }

    if (draw.Blue > maxDraw.Blue)
    {
        return false;
    }

    return true;
}

Draw ParseDraw(string rawDraw)
{
    var drawRegex = new Regex(@"(\d+) (red|green|blue)");
    var parts = rawDraw.Split(",").Select(s => drawRegex.Matches(s)[0]);
    var red = 0;
    var green = 0;
    var blue = 0;
    foreach (var part in parts)
    {
        int amount = int.Parse(part.Groups[1].Value);
        switch (part.Groups[2].Value)
        {
            case "red":
                red = amount;
                break;
            case "green":
                green = amount;
                break;
            case "blue":
                blue = amount;
                break;
        }
    }

    return new Draw(red, green, blue);
}

int Part1(List<string> lines)
{
    var maxDraw = new Draw(12, 13, 14);

    List<int> possibleGames = new List<int>();
    foreach (var l in lines)
    {
        Match match = LineRegex().Matches(l)[0];
        var gameNumber = int.Parse(match.Groups[1].Value);
        var rawDraws = new List<string>(match.Groups[2].Value.Split(";"));
        var impossibleDraws = rawDraws.Select(ParseDraw)
            .Where(d => !IsDrawPossible(d, maxDraw)).ToList().Count;
        if (impossibleDraws == 0)
        {
            possibleGames.Add(gameNumber);
        }
    }

    return possibleGames.Sum();
}

int Part2(List<string> lines)
{
    var products = new List<int>();
    foreach (var l in lines)
    {
        Match match = LineRegex().Matches(l)[0];
        var rawDraws = new List<string>(match.Groups[2].Value.Split(";"));
        var minDraw = rawDraws.Select(ParseDraw)
            .Aggregate(new Draw(0, 0, 0), (acc, n) =>
                new Draw(Math.Max(acc.Red, n.Red), Math.Max(acc.Green, n.Green), Math.Max(acc.Blue, n.Blue)));
        products.Add(minDraw.Red * minDraw.Green * minDraw.Blue);
    }

    return products.Sum();
}

var lines = new List<string>(File.ReadAllLines("input.txt"));
var part1 = Part1(lines);
var part2 = Part2(lines);

Console.WriteLine($"Part 1: {part1}, Part 2: {part2}");

Runner.Benchmark(delegate
{
    Part1(lines);
    Part2(lines);
}, "Day 2");

partial class Program
{    
    [GeneratedRegex(@"Game (\d+): (.*)")]
    public static partial Regex LineRegex();
}

internal class Draw(int red, int green, int blue)
{
    public int Red { get; } = red;
    public int Green { get; } = green;
    public int Blue { get; } = blue;
}