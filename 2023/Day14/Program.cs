using System.Text;
using Utilities;
#pragma warning disable CS8321 // Local function is declared but never used

// TODO: Profile and fix reverse
void DebugPrint(List<string> lines)
{
    foreach (var line in lines)
    {
        Console.WriteLine(line);
    }
}

string TiltLine(string line)
{
    // You can think of each row or column as n stops (either the end of the string or a #)
    // which means we can just use sort to sort the rocks after splitting into our stops
    
    // Initial more elegant but slower code
    // return string.Join("#", line.Split("#").Select(l =>
    // {
    //     int stones = l.Count(c => c == 'O');
    //     return new string('.', l.Length - stones) + new string('O', stones);
    // }));
    
    var builder = new StringBuilder();

    var parts = line.Split('#');
    for (int i = 0; i < parts.Length; i++)
    {
        if (i > 0)
        {
            builder.Append('#');
        }

        var part = parts[i];
        int stones = part.Count(c => c == 'O');

        builder.Append('.', part.Length - stones).Append('O', stones);
    }

    var joined = builder.ToString();
    return joined;
}

List<string> Rotate(List<string> lines, bool clockwise)
{
    if (clockwise)
    {
        var crotated = Enumerable
            .Range(0, lines[0].Length)
            .Select(x => new string(lines.Select(y => y[x]).Reverse().ToArray()));
        return crotated.ToList();
    }
    
    var ccrotated = Enumerable
        .Range(0, lines[0].Length)
        .Select(x => new string(lines.Select(y => y[lines[0].Length - x - 1]).ToArray()));
    return ccrotated.ToList();
}

List<string> Transpose(IReadOnlyList<string> lines)
{
    var transposed = Enumerable
        .Range(0, lines[0].Length)
        .Select(x => new string(lines.Select(y => y[x]).ToArray()));
    return transposed.ToList();
}

List<string> Reverse(IEnumerable<string> lines)
{
    return lines.Reverse().ToList();
}

List<string> TiltBoard(IEnumerable<string> lines)
{
    return lines.Select(TiltLine).ToList();
}

List<string> TiltNorth(IEnumerable<string> lines)
{
    return Reverse(TiltSouth(Reverse(lines)));
}

List<string> TiltEast(IEnumerable<string> lines)
{
    return TiltBoard(lines);
}

List<string> TiltWest(IReadOnlyList<string> lines)
{
    return Transpose(TiltNorth(Transpose(lines)));
}

List<string> TiltSouth(IReadOnlyList<string> lines)
{
    return Transpose(TiltEast(Transpose(lines)));
}

long CalculateLoad(IReadOnlyCollection<string> lines)
{
    var len = lines.Count;
    return lines.Select((s, i) => s.Count(c => c == 'O') * (len-i)).Sum();
}

long Part1(IEnumerable<string> lines)
{
    var rolled = TiltNorth(lines);
    return CalculateLoad(rolled);
}

long Part2(List<string> lines)
{
    // Collect the index of all previous states until we find a cycle
    var previousStates = new Dictionary<string, long>();
    var period = 0L;
    long start;
    while (true)
    {
        //lines = TiltEast(TiltSouth(TiltWest(TiltNorth(lines))));
        for (var _ = 0; _ < 4; _++)
        {
            lines = TiltBoard(Rotate(lines, true));
        }
        
        var currentState = string.Join('\n', lines);
        if (!previousStates.TryAdd(currentState, period))
        {
            start = previousStates[currentState];
            period = period - start;
            break;
        }
        
        period++;
    }

    var cycle = new string[previousStates.Count];
    foreach (var (value, key) in previousStates)
    {
        cycle[key] = value;
    }

    const int totalTime = 1000000000;
    var cycleIndex = start + (totalTime - start) % period - 1;
    var finalState = cycle[cycleIndex];
    
    return CalculateLoad(finalState.Split('\n').ToList());
}

var lines = new List<string>(File.ReadAllLines("input.txt"));
var part1 = Part1(lines);
var part2 = Part2(lines);

Console.WriteLine($"Part 1: {part1}, Part 2: {part2}");

Runner.Benchmark(delegate
{
    Part1(lines);
    Part2(lines);
}, "Day 14");