using System.Data;
using Utilities;
using Vec2 = Utilities.VectorUtilities.Vec2;
using Grid = Utilities.VectorUtilities.Grid2D<char>;

List<Vec2> directions =
[
    new Vec2(0, 1),
    new Vec2(0, -1),
    new Vec2(1, 0),
    new Vec2(-1, 0)
];

void DebugPrint(Grid grid, Dictionary<Vec2, long> visited)
{
    for (var x = 0; x <= grid.MaxRows; x++)
    {
        for (var y = 0; y <= grid.MaxCols; y++)
        {
            char c = grid.Get((x, y));
            if (visited.TryGetValue(new Vec2(x, y), out var s))
            {
                if (s % 2 == 0)
                {
                    c = 'O';
                }
            }
            
            Console.Write(c);
        }
        Console.WriteLine("");
    }
}

(long x, long y) Wrap(Vec2 pos, long width, long height)
{
    var (x, y) = (pos.X % height, pos.Y % width);
    if (x < 0)
    {
        x += height;
    }

    if (y < 0)
    {
        y += width;
    }

    return (x, y);
}

long WalkGarden(Grid grid, Vec2 startingPoint, long numSteps)
{
    var toVisit = new Queue<(Vec2 pos, long step)>(2048);
    var visited = new Dictionary<(long, long), long>(2048);
    toVisit.Enqueue((startingPoint, 0));
    while (toVisit.Count > 0)
    {
        var (pos, step) = toVisit.Dequeue();
        if (step > numSteps)
        {
            // No need to process this
            continue;
        }
        
        // Profiled performance and using tuple instead of Vec2 shaves quite a bit of time off
        if (!visited.TryAdd((pos.X, pos.Y), step))
        {
            // Already visited
            continue;
        }

        var neighbors = directions.Select(d => pos + d);
        toVisit.EnqueueRange(neighbors
            .Where(p =>
            {
                var wrapped = Wrap(p, grid.MaxRows+1, grid.MaxCols+1);
                return grid.Get(wrapped) != '#'; // && grid.Contains(wrapped); Not needed because we're infinite
            })
            .Select(p => (p, step+1))
        );
    }

    return visited.Count(tuple => numSteps % 2 == 0 ? tuple.Value % 2 == 0 : tuple.Value % 2 != 0);
}

(Grid grid, Vec2 startingPoint) ParseGrid(IReadOnlyList<string> lines)
{
    Vec2 startingPoint = new Vec2(0, 0);
    var grid = Grid.CreateFromInput(lines, (c, x, y) =>
    {
        if (c == 'S')
        {
            startingPoint = new Vec2(x, y);
        }
        return c;
    });

    return (grid, startingPoint);
}

IEnumerable<long> GetDiffs(IEnumerable<long> input)
{
    // Take pairwise diffs
    return input.Skip(1).Zip(input).Select(t => t.First - t.Second);
}

long Part1(IReadOnlyList<string> lines)
{
    var (grid, startingPoint) = ParseGrid(lines);
    var visited = WalkGarden(grid, startingPoint, 64);
    return visited;
}

long Part2(IReadOnlyList<string> lines)
{
    // I was not able to come up with something that worked. I saw the rhombus but didn't make any
    // quadratic connection. Thanks for reddit for the hints!
    var (grid, startingPoint) = ParseGrid(lines);
    
    var steps = Enumerable.Range(0, 4).Select(i => 65 + i * 131);
    var visited = steps.Select(s => WalkGarden(grid, startingPoint, s)).ToList();

    var diffs1 = GetDiffs(visited);
    var diffs2 = GetDiffs(diffs1);
    var diffs3 = GetDiffs(diffs2); // diffs3 should be 0 if this is quadratic
    
    var quadratic = diffs2.First() / 2;
    var linear = diffs1.First() - quadratic;
    var constant = visited.First();
    const int finalStep = (26501365 - 65) / 131;
    
    return quadratic * finalStep * finalStep + linear * finalStep + constant;
}

var lines = new List<string>(File.ReadAllLines("input.txt"));
var part1 = Part1(lines);
var part2 = Part2(lines);

Console.WriteLine($"Part 1: {part1}, Part 2: {part2}");

Runner.Benchmark(delegate
{
    Part1(lines);
    Part2(lines);
}, "Day 21");