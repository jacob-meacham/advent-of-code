using Utilities;
using Vec3 = Utilities.VectorUtilities.Vec3;

IList<Brick> Parse(IEnumerable<string> lines)
{
    Vec3 ParsePoint(string pt)
    {
        var list = pt.Split(',').Select(int.Parse).ToList();
        return new Vec3(list[0], list[1], list[2]);
    }
    
    return lines.Select(l =>
    {
        var (startStr, endStr) = l.Split('~');
        return new Brick(ParsePoint(startStr), ParsePoint(endStr));
    }).ToList();
}

HashSet<int> DropBrick(Brick brick, int brickIndex, IDictionary<(long x, long y), (long height, int brickIndex)> heightMap)
{
    var maxZ = 1L;
    HashSet<int> supportingBricks = [];
    foreach (var (x, y) in brick.Project2D())
    {
        if (!heightMap.ContainsKey((x, y)))
        {
            continue;
        }
        
        var (height, index) = heightMap[(x, y)];
        if (height < maxZ)
        {
            continue;
        }
            
        if (height == maxZ)
        {
            supportingBricks.Add(index);    
        }
        else
        {
            maxZ = height;
            supportingBricks.Clear();
            supportingBricks.Add(index);
        }
    }

    var newZ = maxZ + brick.End.Z - brick.Start.Z;
    foreach (var (x, y) in brick.Project2D())
    {
        if (!heightMap.TryGetValue((x, y), out var val))
        {
            val.height = 0;
        }
        if (newZ + 1 > val.height)
        {
            heightMap[(x, y)] = (newZ + 1, brickIndex);
        }
    }

    return supportingBricks;
}

long CountGraph(int brickToRemove, List<List<int>> graph)
{
    // Initialize the degree of each node
    var inDegree = Enumerable.Repeat(0, graph.Count).ToList();
    foreach (var node in graph.SelectMany(nodes => nodes))
    {
        inDegree[node] += 1;
    }

    var queue = new Queue<int>(graph.Count);
    queue.Enqueue(brickToRemove);

    var count = -1;
    while (queue.Count > 0)
    {
        count++;
        var brick = queue.Dequeue();
        foreach (var node in graph[brick])
        {
            inDegree[node] -= 1;
            if (inDegree[node] == 0)
            {
                // Found a node that will move
                queue.Enqueue(node);
            }
        }
    }

    return count;
}

long Part1(IEnumerable<string> lines)
{
    var bricks = Parse(lines).OrderBy(b => b.MaxZ).ToList();
    var heightMap = new Dictionary<(long x, long y), (long height, int brickIndex)>();
    
    var supportedBy = bricks
        .Select((brick, index) => (brickIndex: index, supportingBricks: DropBrick(brick, index, heightMap)))
        .ToDictionary(tuple => tuple.brickIndex, tuple => tuple.supportingBricks);

    var unsafeToDisintegrate = supportedBy.Where(pair => pair.Value.Count == 1).Select(pair => pair.Value).ToHashSet();
    return bricks.Count - unsafeToDisintegrate.Count;
}

long Part2(IReadOnlyList<string> lines)
{
    var bricks = Parse(lines).OrderBy(b => b.MaxZ).ToList();
    var heightMap = new Dictionary<(long x, long y), (long height, int brickIndex)>();
    
    var supportedBy = bricks
        .Select((brick, index) => (brickIndex: index, supportingBricks: DropBrick(brick, index, heightMap)))
        .ToDictionary(tuple => tuple.brickIndex, tuple => tuple.supportingBricks);
    
    // Create a graph, do a topological sort and look for nodes whose in-degree falls to zero as we remove nodes
    var graph = Enumerable.Range(0, bricks.Count).Select(_ => new List<int>()).ToList();
    foreach (var (brickIndex, support) in supportedBy)
    {
        foreach (var supportBrick in support)
        {
            graph[supportBrick].Add(brickIndex);    
        }
    }

    return Enumerable.Range(0, bricks.Count).Select(index => CountGraph(index, graph)).Sum();
}

var lines = new List<string>(File.ReadAllLines("input.txt"));
var part1 = Part1(lines);
var part2 = Part2(lines);

Console.WriteLine($"Part 1: {part1}, Part 2: {part2}");

Runner.Benchmark(delegate
{
    Part1(lines);
    Part2(lines);
}, "Day 22");

internal record Brick(Vec3 Start, Vec3 End)
{
    public long MaxZ => Math.Max(Start.Z, End.Z);
    
    internal IEnumerable<(long x, long y)> Project2D()
    {
        for (var x = Start.X; x <= End.X; x++)
        {
            for (var y = Start.Y; y <= End.Y; y++)
            {
                yield return (x, y);
            }
        }
    }
}