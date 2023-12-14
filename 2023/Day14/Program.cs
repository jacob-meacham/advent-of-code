using System.Text;
using Utilities;

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
    return string.Join("#", line.Split("#").Select(l =>
    {
        return new string(l.OrderBy(c => c == 'O' ? 1 : 0).ToArray());
    }));
}

List<string> Rotate(List<string> lines, bool clockwise)
{
    var rotated = new List<string>(lines.Count);
    
    if (clockwise)
    {
        for (int i = 0; i < lines[0].Length; i++)
        {
            var column = new StringBuilder();
            for (int j = lines.Count - 1; j >= 0; j--)
            {
                column.Append(lines[j][i]);
            }
            rotated.Add(column.ToString());
        }
    }
    else
    {
        for (int i = lines[0].Length - 1; i >= 0; i--)
        {
            var column = new StringBuilder();
            for (int j = 0; j < lines.Count; j++)
            {
                column.Append(lines[j][i]);
            }
            rotated.Add(column.ToString());
        }
    }
    
    
    return rotated;
}

List<string> TiltBoard(List<string> lines)
{
    return lines.Select(TiltLine).ToList();
}

long CalculateLoad(List<string> lines)
{
    var len = lines.Count;
    return lines.Select((s, i) => s.Count(c => c == 'O') * (len-i)).Sum();
}

long Part1(List<string> lines)
{
    var rolled = Rotate(TiltBoard(Rotate(lines, true)), false);
    return CalculateLoad(rolled);
}

long Part2(List<string> lines)
{
    // Collect the index of all previous states until we find a cycle
    var previousStates = new Dictionary<string, long>();
    var period = 0L;
    var start = 0L;
    while (true)
    {
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

    var totalTime = 1000000000;
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