using Utilities;
using Vec2 = Utilities.VectorUtilities.Vec2;
#pragma warning disable CS8321 // Local function is declared but never used

void DebugPrint(VectorUtilities.Grid2D<string> grid)
{
    for (var x = 0; x <= grid.MaxRows; x++)
    {
        for (var y = 0; y <= grid.MaxCols; y++)
        {
            var c = grid.Get((x, y));
            Console.Write(c is null ? '.' : '#');
        }
        Console.WriteLine("");
    }
}

// Calculate a polygon's area using the Shoelace formula: https://en.wikipedia.org/wiki/Shoelace_formula
long PolygonArea(IReadOnlyCollection<Vec2> points)
{
    // Could also use an aggregate but I think this is more readable
    var x = points.Select(vec2 => vec2.X).ToList();
    x.Add(points.First().X);
    var y = points.Select(vec2 => vec2.Y).ToList();
    y.Add(points.First().Y);

    var sum = 0L;
    for (var i = 0; i < x.Count - 1; i++)
    {
        sum += x[i] * y[i + 1] - x[i + 1] * y[i];
    }
    
    return (long)(Math.Abs(sum) / 2.0);
}

long Part1(IEnumerable<string> lines)
{
    Vec2 curPos = new(0, 0);
    var walls = new List<(Vec2 start, Vec2 dir, int length, string color)>();
    
    var (minX, maxX) = (0L, 0L);
    var (minY, maxY) = (0L, 0L);
    
    foreach (var line in lines)
    {
        var lineParts = line.Split(' ');
        var dir = lineParts[0] switch
        {
            "U" => Vec2.North,
            "D" => Vec2.South,
            "L" => Vec2.West,
            "R" => Vec2.East,
            _ => throw new ArgumentOutOfRangeException()
        };

        var distance = int.Parse(lineParts[1]);
        walls.Add((curPos, dir, distance, lineParts[2]));
        
        minX = Math.Min(minX, curPos.X);
        maxX = Math.Max(maxX, curPos.X + 1);
        
        minY = Math.Min(minY, curPos.Y);
        maxY = Math.Max(maxY, curPos.Y + 1);
        
        curPos += dir * distance;
    }

    var (height, width) = (maxX - minX, maxY - minY);
    var cells = new string[height, width];
    var grid = new VectorUtilities.Grid2D<string>(cells);
    
    foreach (var (start, dir, length, color) in walls)
    {
        var newStart = start + (-minX, -minY);
        for (var i = 0; i <= length; i++)
        {
            grid.Get(newStart) = color;
            newStart += dir;
        }
    }

    // Use ray casting to determine inside vs. outside for each point
    var inside = false;
    for (var x = 0; x <= grid.MaxRows; x++)
    {
        for (var y = 0; y <= grid.MaxCols; y++)
        {
            var c = grid.Get((x, y));
            if (c is null)
            {
                if (inside)
                {
                    grid.Get((x, y)) = "";
                }
                continue;
            }

            if (x + 1 <= grid.MaxRows && grid.Get((x + 1, y)) is not null)
            {
                inside = !inside;
            }
        }
    }

    return grid.GetCells().Count(tuple => tuple.cell is not null);
}

// Shoelace formula
long Part2(IEnumerable<string> lines)
{
    Vec2 curPos = new(0, 0);
    var points = new List<Vec2>();

    long boundaryPoints = 0;
    foreach (var line in lines)
    {
        var instruction = line.Split("#")[1];
        
        var dir = instruction[5] switch
        {
            '0' => Vec2.East,
            '1' => Vec2.North,
            '2' => Vec2.West,
            '3' => Vec2.South,
            _ => throw new ArgumentOutOfRangeException(nameof(lines))
        };

        var distance = Convert.ToInt64(instruction.Substring(0,5), 16);
        
        boundaryPoints += distance;
        curPos += dir * distance;
        
        points.Add(curPos);
    }

    var totalArea = PolygonArea(points);
    
    // https://en.wikipedia.org/wiki/Pick's_theorem
    // In particular, A = i + b/2 - 1. We have A and b, and would like i
    var interiorArea = totalArea + 1 - boundaryPoints / 2;
    return interiorArea + boundaryPoints;
}

var lines = new List<string>(File.ReadAllLines("input.txt"));
var part1 = Part1(lines);
var part2 = Part2(lines);

Console.WriteLine($"Part 1: {part1}, Part 2: {part2}");

Runner.Benchmark(delegate
{
    Part1(lines);
    Part2(lines);
}, "Day 18");