using Utilities;
using Vec2 = Utilities.TwoDUtilities.Vec2;
#pragma warning disable CS8321 // Local function is declared but never used

void DebugPrint(Cell[,] grid, Vec2 initialPos)
{
    for (var x = 0; x < grid.GetLength(0); x++)
    {
        for (var y = 0; y < grid.GetLength(1); y++)
        {
            if (initialPos.X == x && initialPos.Y == y)
            {
                Console.Write('E');    
            }
            else
            {
                Console.Write(grid[x, y].IsHit() ? '#' : '.');    
            }
        }
        Console.WriteLine("");
    }
}

bool OutOfBounds(Cell[,] grid, Vec2 pos)
{
    return pos.X < 0 || pos.Y < 0 || pos.X >= grid.GetLength(0) || pos.Y >= grid.GetLength(1);
}

// This function casts the ray as far as it can in a single direction.
// It marks the grid as it goes and short circuits if it hits an already explored path
Vec2 CastRay(Cell[,] grid, Vec2 pos, Vec2 direction, out Cell? cell)
{
    while (true)
    {
        pos += direction;
        if (OutOfBounds(grid, pos))
        {
            cell = null;
            return pos;
        }

        cell = grid[pos.X, pos.Y];
        if (cell.IsHitFromDirection(direction))
        {
            // Already explored this path
            cell = null;
            return pos;
        }

        cell.HitFromDirection(direction);
        var cellType = cell.CellType;
        if ((cellType == '-' && direction.X != 0) || (cellType == '|' && direction.Y != 0) || cellType == '/' ||
            cellType == '\\')
        {
            // Encountered an interesting cell, so let's goooooooo
            return pos;
        }
    }
}

// TODO: Could memoize this in some way
long Energize(Cell[,] grid, Vec2 startingPosition, Vec2 initialDirection)
{
    var rays = new Stack<(Vec2 pos, Vec2 direction)>(512);
    rays.Push((startingPosition, initialDirection));
    
    while (rays.Count > 0)
    {
        var (pos, direction) = rays.Pop();
        var hitPos = CastRay(grid, pos, direction, out var hitCell);
        if (hitCell is null)
        {
            continue;   
        }
        
        // TODO: I'm exploring paths that I don't need to
        switch (hitCell.CellType)
        {
            case '-':
                rays.Push((hitPos, new Vec2(0, -1)));
                rays.Push((hitPos, new Vec2(0, 1)));
                break;
            case '|':
                rays.Push((hitPos, new Vec2(-1, 0)));
                rays.Push((hitPos, new Vec2(1, 0)));
                break;
            case '/':
                rays.Push((hitPos, new Vec2(-direction.Y, -direction.X)));
                break;
            case '\\':
                rays.Push((hitPos, new Vec2(direction.Y, direction.X)));
                break;
        }
    }
    
    var numHit = 0L;
    for (var x = 0; x < grid.GetLength(0); x++)
    {
        for (var y = 0; y < grid.GetLength(1); y++)
        {
            if (grid[x, y].IsHit())
            {
                numHit++;
            }
        }
    }
    return numHit;
}

Cell[,] ParseGrid(IReadOnlyList<string> lines)
{
    Cell[,] grid = new Cell[lines.Count,lines[0].Length];
    for (var x = 0; x < grid.GetLength(0); x++)
    {
        for (var y = 0; y < grid.GetLength(1); y++)
        {
            grid[x, y] = new Cell(lines[x][y]);
        }
    }

    return grid;
}

void Reset(Cell[,] grid)
{
    for (var x = 0; x < grid.GetLength(0); x++)
    {
        for (var y = 0; y < grid.GetLength(1); y++)
        {
            grid[x, y].Reset();
        }
    }
}

long Part1(IReadOnlyList<string> lines)
{
    var grid = ParseGrid(lines);
    return Energize(grid, new Vec2(0, -1), new Vec2(0, 1));
}

long Part2(IReadOnlyList<string> lines)
{
    var grid = ParseGrid(lines);
    
    var energizedOutputs = new List<long>();
    for (var x = 0; x < lines.Count; x++)
    {
        energizedOutputs.Add(Energize(grid, new Vec2(x, -1), new Vec2(0, 1)));
        Reset(grid);
        
        energizedOutputs.Add(Energize(grid, new Vec2(x, lines[0].Length), new Vec2(0, -1)));
        Reset(grid);
    }
    
    for (var y = 0; y < lines[0].Length; y++)
    {
        energizedOutputs.Add(Energize(grid, new Vec2(-1, y), new Vec2(1, 0)));
        Reset(grid);
        
        energizedOutputs.Add(Energize(grid, new Vec2(lines.Count, y), new Vec2(-1, 0)));
        Reset(grid);
    }
    
    return energizedOutputs.Max();
}

var lines = new List<string>(File.ReadAllLines("input.txt"));
var part1 = Part1(lines);
var part2 = Part2(lines);

Console.WriteLine($"Part 1: {part1}, Part 2: {part2}");

Runner.Benchmark(delegate
{
    Part1(lines);
    Part2(lines);
}, "Day 16");

[Flags]
internal enum Direction
{
    NegX = 1,
    PosX = 2,
    NegY = 4,
    PosY = 8
}

internal class Cell(char cellType)
{
    public char CellType { get; } = cellType; 
    
    private Direction _isHit;

    public void HitFromDirection(Vec2 v)
    {
        switch (v)
        {
            case { X: 1 }:
                _isHit |= Direction.PosX;
                break;
            case { X: -1 }:
                _isHit |= Direction.NegX;
                break;
            case { Y: 1 }:
                _isHit |= Direction.PosY;
                break;
            case { Y: -1 }:
                _isHit |= Direction.NegY;
                break;
        }
    }

    public bool IsHit()
    {
        return _isHit != 0;
    }

    public bool IsHitFromDirection(Vec2 v)
    {
        return v switch
        {
            { X: 1 } => (_isHit & Direction.PosX) != 0,
            { X: -1 } => (_isHit & Direction.NegX) != 0,
            { Y: 1 } => (_isHit & Direction.PosY) != 0,
            { Y: -1 } => (_isHit & Direction.NegY) != 0,
            _ => throw new ArgumentOutOfRangeException(nameof(v))
        };
    }

    public void Reset()
    {
        _isHit = 0;
    }
}