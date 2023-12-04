using Utilities;

static int GetDigits(in string line)
{
    return int.Parse("" + line.First(Char.IsDigit) + line.Last(Char.IsDigit));
}

static string Sanitize(in string line)
{
    // Due to possible overlaps, we need to also replace any characters that might be used in the overlap
    return line.Replace("one", "o1e")
        .Replace("two", "t2")
        .Replace("three", "t3e")
        .Replace("four", "4")
        .Replace("five", "5e")
        .Replace("six", "s6")
        .Replace("seven", "s7n")
        .Replace("eight", "e8t")
        .Replace("nine", "n9e");
}

static int Part1(in List<String> lines)
{
    return lines.Select(l => GetDigits(l)).Sum();
}

static int Part2(in List<String> lines)
{
    return lines.Select(l => Sanitize(l)).Select(l => GetDigits(l)).Sum();
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
}, "Day 1");