using System.Text.RegularExpressions;
using Utilities;

(string From, string To, List<MappingRange> Mappings) ParseMappings(string[] lines)
{
    var match = MappingNameRegex().Matches(lines[0])[0];
    var fromName = match.Groups[1].Value;
    var toName = match.Groups[2].Value;
    var mappingRanges = lines.Skip(1)
        .Select(line =>
        {
            var parts = line.Split(" ");
            return new MappingRange(long.Parse(parts[0]), long.Parse(parts[1]), long.Parse(parts[2]));
        })
        .OrderBy(mr => mr.From.Min)
        .ToList();

    return (fromName, toName, mappingRanges);
}

long Part1(List<string> lines)
{
    var sections = string.Join("\n", lines).Split("\n\n");
    var types = sections[0].Split(": ")[1].Split(" ").Select(long.Parse).ToList();
    
    var mappingDictionary = new Dictionary<string, Mapping>();
    foreach (var section in sections.Skip(1))
    {
        var (fromName, toName, mappingRanges) = ParseMappings(section.Split("\n"));
        mappingDictionary[fromName] = new Mapping(toName, mappingRanges);
    }
    
    var type = "seed";
    while (true)
    {
        var mapping = mappingDictionary[type];
        types = types.Select(mapping.MapValue).ToList();
        
        type = mapping.To;
        if (type == "location")
        {
            break;
        }
    }
    
    return types.Min();
}

long Part2(List<String> lines)
{
    var sections = string.Join("\n", lines).Split("\n\n");
    var rawSeeds = sections[0].Split(": ")[1].Split(" ").Select(long.Parse).ToList();
    
    // Stores the mapping for the original seed range -> ranges that have been mapped
    var seedRanges = Enumerable.Range(0, rawSeeds.Count / 2)
        .Select(i =>
        {
            long start = rawSeeds[i * 2];
            long length = rawSeeds[i * 2 + 1];
            var range = new Range(start, start + length - 1);
            return new MappingRange(range, range);
        });
    
    var mappingDictionary = new Dictionary<string, Mapping>();
    foreach (var section in sections.Skip(1))
    {
        var (fromName, toName, mappingRanges) = ParseMappings(section.Split("\n"));
        mappingDictionary[fromName] = new Mapping(toName, mappingRanges);
    }
    
    var type = "seed";
    while (true)
    {
        var mapping = mappingDictionary[type];
        seedRanges = seedRanges.SelectMany(n => mapping.MapRanges(n)).ToList();
        
        type = mapping.To;
        if (type == "location")
        {
            break;
        }
    }

    var range = seedRanges.OrderBy(mr => mr.To.Min).First();
    return range.To.Min;
}

var lines = new List<string>(File.ReadAllLines("input.txt"));
var part1 = Part1(lines);
var part2 = Part2(lines);

Console.WriteLine($"Part 1: {part1}, Part 2: {part2}");

Runner.Benchmark(delegate
{
    Part1(lines);
    Part2(lines);
}, "Day 5");

partial class Program
{
    [GeneratedRegex("([a-zA-Z]+)-to-([a-zA-Z]+) map:")]
    private static partial Regex MappingNameRegex();
}

internal class Mapping(string to, List<MappingRange> ranges)
{
    private readonly List<MappingRange> _ranges = ranges;
    public string To { get; } = to;

    public List<MappingRange> MapRanges(MappingRange range)
    {
        var remainingRange = range;
        var newRanges = new List<MappingRange>();
        
        // ranges are stored in sorted order
        foreach (var mappedRange in _ranges)
        {
            var (first, rest) = MappingRange.Split(remainingRange, mappedRange.From.Min);
            if (first.Length > 0)
            {
                newRanges.Add(first);
            }

            var (middle, last) = MappingRange.Split(rest, mappedRange.From.Max);
            if (middle.Length > 0)
            {
                newRanges.Add(
                    new MappingRange(
                        new Range(mappedRange.GetMappingForValue(middle.To.Min), mappedRange.GetMappingForValue(middle.To.Max)), 
                        new Range(middle.From.Min, middle.From.Max)));
            }

            remainingRange = last;
            if (remainingRange.Length <= 0)
            {
                break;
            }
        }

        if (remainingRange.Length > 0)
        {
            newRanges.Add(remainingRange);
        }

        return newRanges;
    }

    public long MapValue(long value)
    {
        return _ranges.Aggregate(value, (acc, n) =>
        {
            if (acc != value)
            {
                // Already mapped
                return acc;
            }

            if (n.IsValueInMapping(value))
            {
                return n.GetMappingForValue(value);
            }

            return acc;
        });
    }
}

internal class Range(long min, long max)
{
    public long Min { get; } = min;
    public long Max { get; } = max;
}
internal class MappingRange
{
    public Range To { get; }
    public Range From { get; }
    public long Length { get; }
    
    public bool IsValueInMapping(long value)
    {
        return value >= From.Min && value <= From.Max;
    }

    public long GetMappingForValue(long value)
    {
        return value - From.Min + To.Min;
    }

    public MappingRange(long to, long from, long length)
    {
        To = new Range(to, to + length);
        From = new Range(from, from + length);
        Length = length;
    }
    
    public MappingRange(Range to, Range from)
    {
        To = to;
        From = from;
        Length = From.Max - From.Min;
    }

    public static (MappingRange left, MappingRange right) Split(MappingRange original, long splitPoint)
    {
        long leftLength =  Math.Min(original.Length, Math.Max(0, splitPoint - original.To.Min));
        long rightLength = original.Length - leftLength;
        return (new MappingRange(original.To.Min, original.From.Min, leftLength),
                new MappingRange(original.To.Min + leftLength, original.From.Min + leftLength,
                    rightLength)
            );
    }
}