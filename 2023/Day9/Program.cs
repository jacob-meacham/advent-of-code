using Utilities;

List<IEnumerable<long>> GetSequences(IEnumerable<long> sequence)
{
    var sequences = new List<IEnumerable<long>>();
    sequences.Add(sequence);
    while (true)
    {
        sequence = sequence.Pairwise().Select(tuple => tuple.Item2 - tuple.Item1);
        sequences.Add(sequence);

        if (sequence.Count(l => l != 0) == 0)
        {
            break;
        }
    }

    return sequences;
}

long GetNextInSequence(IEnumerable<long> sequence)
{
    var sequences = GetSequences(sequence);

    List<long> lasts = sequences.Select(l => l.Last()).Reverse().ToList();
    var toAdd = 0L;
    foreach (var l in lasts.Skip(1))
    {
        toAdd = l + toAdd;
    }
    
    return toAdd;
}

long GetPreviousInSequence(IEnumerable<long> sequence)
{
    var sequences = GetSequences(sequence);

    List<long> firsts = sequences.Select(l => l.First()).Reverse().ToList();
    var toAdd = 0L;
    foreach (var l in firsts.Skip(1))
    {
        toAdd = l - toAdd;
    }
    
    return toAdd;
}

long Part1(List<string> lines)
{
    var values = new List<long>();
    foreach (var l in lines)
    {
        var next = GetNextInSequence(l.Split(" ").Select(long.Parse));
        values.Add(next);
    }
    return values.Sum();
}

long Part2(List<string> lines)
{
    var values = new List<long>();
    foreach (var l in lines)
    {
        var next = GetPreviousInSequence(l.Split(" ").Select(long.Parse));
        values.Add(next);
    }
    return values.Sum();
}

var lines = new List<string>(File.ReadAllLines("input.txt"));
var part1 = Part1(lines);
var part2 = Part2(lines);

Console.WriteLine($"Part 1: {part1}, Part 2: {part2}");

Runner.Benchmark(delegate
{
    Part1(lines);
    Part2(lines);
}, "Day 9");