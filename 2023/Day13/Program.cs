using Utilities;

IEnumerable<Grid> ParseGrids(IEnumerable<string> lines)
{
    var grids = string.Join("\n", lines)
        .Split("\n\n")
        .Select(l => l.Split("\n"))
        .Select(ParseGrid);

    return grids;
}

Grid ParseGrid(string[] gridLines)
{
    // Turn each row / column into a bitmask
    // starting from the 1st row/col look for reflections
    
    var rows = new List<long>();
    var cols = new List<long>();
    
    // There is probably a fancier way to do this that doesn't require two passes
    foreach (var gridLine in gridLines)
    {
        long row = 0;
        for (var j = 0; j < gridLines[0].Length; j++)
        {
            row = (row << 1) + (gridLine[j] == '#' ? 1 : 0);
        }
        rows.Add(row);
    }
    
    for (var j = 0; j < gridLines[0].Length; j++)
    {
        long col = 0;
        foreach (var gridLine in gridLines)
        {
            col = (col << 1) + (gridLine[j] == '#' ? 1 : 0);
        }
        cols.Add(col);
    }

    return new Grid(rows, cols);
}

int CountDifferentBits(long a, long b)
{
    var diff = a ^ b;
    return System.Numerics.BitOperations.PopCount((ulong)diff);
}

// Returns index of reflection in the input and 0 if no reflection is found
long FindReflection(List<long> input, long allowedDifferences)
{
    var differences = new List<(long differences, long index)>();
    for (var i = 1; i < input.Count; i++)
    {
        var numDifferences = 0;
        var numToCheck = Math.Min(i, input.Count - i);
        for (var j = 0; j < numToCheck; j++)
        {
            numDifferences += CountDifferentBits(input[i + j], input[i - j - 1]);
        }
        
        differences.Add((numDifferences, i));
    }

    // Assumes there is only ever one line of reflection
    return differences.Where((tuple => tuple.differences == allowedDifferences)).FirstOrDefault((0,0)).index;
}

long GetReflectionScore(Grid grid, long allowedDifferences)
{
    var verticalReflection = FindReflection(grid.Columns, allowedDifferences);
    if (verticalReflection > 0)
    {
        return verticalReflection;
    }

    var horizontalReflection = FindReflection(grid.Rows, allowedDifferences);
    return 100 * horizontalReflection;
}

long Part1(IEnumerable<string> lines)
{
    var grids = ParseGrids(lines);
    return grids.Select(g => GetReflectionScore(g, 0)).Sum();
}

long Part2(IEnumerable<string> lines)
{
    var grids = ParseGrids(lines);
    return grids.Select(g => GetReflectionScore(g, 1)).Sum();
}

var lines = new List<string>(File.ReadAllLines("input.txt"));
var part1 = Part1(lines);
var part2 = Part2(lines);

Console.WriteLine($"Part 1: {part1}, Part 2: {part2}");

Runner.Benchmark(delegate
{
    Part1(lines);
    Part2(lines);
}, "Day 13");

internal class Grid(List<long> rows, List<long> cols)
{
    public List<long> Rows { get; } = rows;
    public List<long> Columns { get; } = cols;
}