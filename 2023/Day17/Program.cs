using Utilities;
using Vec2 = Utilities.TwoDUtilities.Vec2;
using Grid = Utilities.TwoDUtilities.Grid<int>;

long Solve(TwoDUtilities.AStarSolver<Crucible> solver, Grid grid)
{
    var path = solver.Solve([
        new Crucible(new Vec2(0, 0), new Vec2(1, 0), 0),
        new Crucible(new Vec2(0, 0), new Vec2(0, 1), 0)
    ], new Crucible(new Vec2(grid.MaxRows, grid.MaxCols), new Vec2(0, 0), 0));
        
    return path!.Skip(1).Aggregate(0, (n, crucible) => n + grid.Get(crucible.Pos));
}

long Part1(IReadOnlyList<string> lines)
{
    var grid = Grid.CreateFromInput(lines, (c, _, _) => int.Parse(c.ToString()));
    return Solve(new CrucibleAStarSolver(grid), grid);
}

long Part2(IReadOnlyList<string> lines)
{
    var grid = Grid.CreateFromInput(lines, (c, _, _) => int.Parse(c.ToString()));
    return Solve(new UltraCrucibleAStarSolver(grid), grid);
}

var lines = new List<string>(File.ReadAllLines("input.txt"));
var part1 = Part1(lines);
var part2 = Part2(lines);

Console.WriteLine($"Part 1: {part1}, Part 2: {part2}");

Runner.Benchmark(delegate
{
    Part1(lines);
    Part2(lines);
}, "Day 17", 3);

internal record Crucible(Vec2 Pos, Vec2 Direction, int Distance);

internal class UltraCrucibleAStarSolver : CrucibleAStarSolver
{
    internal UltraCrucibleAStarSolver(Grid grid) : base(grid) { }
    
    protected override IEnumerable<Crucible> GetNeighbors(Crucible current)
    {
        var neighbors = new List<Crucible>();
        if (current.Distance < 4)
        {
            // Must move in a straight line
            neighbors.Add(current with { Pos = current.Pos + current.Direction, Distance = current.Distance + 1 });
        }
        else
        {
            neighbors = GetNeighborsMaxDistance(current, 10);
        }
        
        return neighbors.Where(c => Grid.Contains(c.Pos));
    }
    
    protected override bool IsGoal(Crucible node, Crucible goal)
    {
        return node.Pos == goal.Pos && node.Distance >= 4;
    }
}

// TODO: Optimize by getting cost inline + not storing so much state in the pathMap + potentially use a string key
internal class CrucibleAStarSolver(Grid grid) : TwoDUtilities.AStarSolver<Crucible>
{
    protected readonly Grid Grid = grid;
    
    protected List<Crucible> GetNeighborsMaxDistance(Crucible current, int distance)
    {
        List<Crucible> neighbors = (current.Direction, current.Distance == distance) switch
        {
            // East/West
            ((0, 1) or (0, -1), false) =>
            [
                // continue in current direction. Can't backtrack
                new Crucible(current.Pos + current.Direction, current.Direction, current.Distance + 1),
                new Crucible(current.Pos + Vec2.North, Vec2.North, 1),
                new Crucible(current.Pos + Vec2.South, Vec2.South, 1)
            ],
            ((0, 1) or (0, -1), true) =>
            [
                new Crucible(current.Pos + Vec2.North, Vec2.North, 1),
                new Crucible(current.Pos + Vec2.South, Vec2.South, 1)
            ],
            // North/South
            ((-1, 0) or (1, 0), false) =>
            [
                // continue in current direction. Can't backtrack
                new Crucible(current.Pos + current.Direction, current.Direction, current.Distance + 1),
                new Crucible(current.Pos + Vec2.East, Vec2.East, 1),
                new Crucible(current.Pos + Vec2.West, Vec2.West, 1)
            ],
            ((-1, 0) or (1, 0), true) =>
            [
                new Crucible(current.Pos + Vec2.East, Vec2.East, 1),
                new Crucible(current.Pos + Vec2.West, Vec2.West, 1)
            ],
            _ => throw new ArgumentOutOfRangeException()
        };

        return neighbors;
    }
    
    protected override IEnumerable<Crucible> GetNeighbors(Crucible current)
    {

        var neighbors = GetNeighborsMaxDistance(current, 3);
        return neighbors.Where(c => Grid.Contains(c.Pos));
    }

    protected override long GetHeuristic(Crucible node, Crucible goal)
    {
        return TwoDUtilities.ManhattanDistance(node.Pos, goal.Pos);
    }

    protected override long GetCost(Crucible current, Crucible neighbor)
    {
        return Grid.Get(neighbor.Pos);
    }

    protected override bool IsGoal(Crucible node, Crucible goal)
    {
        return node.Pos == goal.Pos;
    }
}