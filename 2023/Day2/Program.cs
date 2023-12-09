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

Draw ParseDraw(String rawDraw)
{
    Regex drawRegex = new Regex(@"(\d+) (red|green|blue)");
    var parts = rawDraw.Split(",").Select(s => drawRegex.Matches(s)[0]);
    int red = 0;
    int green = 0;
    int blue = 0;
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

int Part1(List<String> lines)
{
    Regex lineRegex = new Regex(@"Game (\d+): (.*)");
    Draw maxDraw = new Draw(12, 13, 14);
    
    List<int> possibleGames = new List<int>();
    foreach (var l in lines)
    {
        Match match = lineRegex.Matches(l)[0];
        var gameNumber = int.Parse(match.Groups[1].Value);
        List<String> rawDraws = new List<String>(match.Groups[2].Value.Split(";"));
        var impossibleDraws = rawDraws.Select(rd => ParseDraw(rd))
            .Where(d => !IsDrawPossible(d, maxDraw)).ToList().Count;
        if (impossibleDraws == 0)
        {
            possibleGames.Add(gameNumber);
        }
    }

    return possibleGames.Sum();
}

int Part2(List<String> lines)
{
    Regex lineRegex = new Regex(@"Game (\d+): (.*)");
    
    List<int> products = new List<int>();
    foreach (var l in lines)
    {
        Match match = lineRegex.Matches(l)[0];
        var gameNumber = int.Parse(match.Groups[1].Value);
        List<String> rawDraws = new List<String>(match.Groups[2].Value.Split(";"));
        var minDraw = rawDraws.Select(rd => ParseDraw(rd))
            .Aggregate(new Draw(0, 0, 0), (acc, n) => 
                new Draw(Math.Max(acc.Red, n.Red), Math.Max(acc.Green, n.Green), Math.Max(acc.Blue, n.Blue)));
        products.Add(minDraw.Red*minDraw.Green*minDraw.Blue);
    }

    return products.Sum();
}

var lines = new List<string>(File.ReadAllLines("input.txt"));
var part1 = Part1(lines);
var part2 = Part2(lines);

Console.WriteLine($"Part 1: {part1}, Part 2: {part2}");

// TODO: Some way of doing this for all of them
Runner.Benchmark(delegate
{
    Part1(lines);
    Part2(lines);
}, "Day 2");

public class Draw
{
    public int Red { get; }
    public int Green { get; }
    public int Blue { get; }

    public Draw(int red, int green, int blue)
    {
        Red = red;
        Green = green;
        Blue = blue;
    }
}
