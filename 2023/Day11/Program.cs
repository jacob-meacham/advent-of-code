using Utilities;

long Solve(bool[,] map, List<string> lines, long totalExpansion)
{
    var emptyRows = Enumerable.Repeat(totalExpansion, map.GetLength(0)).ToList();
    var emptyCols = Enumerable.Repeat(totalExpansion, map.GetLength(1)).ToList();
    for (int x = 0; x < lines.Count; x++)
    {
        var line = lines[x].ToCharArray();
        for (int y = 0; y < line.Length; y++)
        {
            if (line[y] == '#')
            {
                emptyRows[x] = 0;
                emptyCols[y] = 0;
                map[x, y] = true;
            }
            else
            {
                map[x, y] = false;
            }
        }
    }
    
    // Do a scanl on emptyRows and emptyCols so that they have the cumulative number of emptyRows/cols up to that point
    emptyRows = emptyRows.ScanL(0L, (acc, n) => acc + n).ToList();
    emptyCols = emptyCols.ScanL(0L, (acc, n) => acc + n).ToList();
    var galaxyLocations = new List<(long x, long y)>();
    for (int x = 0; x < map.GetLength(0); x++)
    {
        for (int y = 0; y < map.GetLength(1); y++)
        {
            if (map[x, y])
            {
                galaxyLocations.Add((x + emptyRows[x], y + emptyCols[y]));
            }
        }
    }
    
    var numGalaxies = galaxyLocations.Count;
    var pairs = galaxyLocations.Select((galaxy, index) => (galaxy, galaxyLocations.Slice(index, numGalaxies - index)));
    var totalSum = 0L;
    foreach (var (galaxy, others) in pairs)
    {
        totalSum += others.Select(o => TwoDUtilities.ManhattanDistance(galaxy, o)).Sum();
    }
    
    return totalSum;
}

long Part1(bool[,] map, List<string> lines)
{
    return Solve(map, lines, 1);
}

long Part2(bool[,] map, List<string> lines)
{
    return Solve(map, lines, 1000000-1);
}

var lines = new List<string>(File.ReadAllLines("input.txt"));
var map = new bool[lines.Count, lines[0].Length];
//var part1 = Part1(map, lines);
var part1 = 0;
var part2 = Part2(map, lines);

Console.WriteLine($"Part 1: {part1}, Part 2: {part2}");

// Runner.Benchmark(delegate
// {
//     Part1(map, lines);
//     Part2(map, lines);
// }, "Day 11");