using Utilities;
using Grid = Utilities.VectorUtilities.Grid2D<char>;
using Vec2 = Utilities.VectorUtilities.Vec2;
// ReSharper disable AccessToModifiedClosure

List<Vec2> directions =
[
    new Vec2(0, 1),
    new Vec2(0, -1),
    new Vec2(1, 0),
    new Vec2(-1, 0)
];

void DebugPrint(Grid grid, List<Vec2> longestPath)
{
    foreach (var pos in longestPath)
    {
        grid.Get(pos) = 'O';
    }
    
    for (var x = 0; x <= grid.MaxRows; x++)
    {
        for (var y = 0; y <= grid.MaxCols; y++)
        {
            Console.Write(grid.Get((x, y)));
        }
        Console.WriteLine("");
    }
}

bool CanMove(char c, Vec2 from, Vec2 to)
{
    var direction = from - to;
    return c switch
    {
        '^' => direction.X != -1,
        'v' => direction.X != 1,
        '<' => direction.Y != -1,
        '>' => direction.Y != 1,
        '.' => true,
        _ => false
    };
}

bool IsSlope(char slope)
{
    return slope != '.' && slope != '#';
}

// Another graph problem!
long Part1(IReadOnlyList<string> lines)
{
    var grid = Grid.CreateFromInput(lines, (c, _, _) => c);
    var start = new Vec2(0, lines[0].IndexOf('.'));
    var goal = new Vec2(grid.MaxRows, lines[^1].IndexOf('.'));
    
    var graph = new Graph<Vec2>();
    var startNode = graph.AddNode(start);
    
    // Create our graph
    var visited = new Dictionary<Vec2, Node<Vec2>>();
    var searchStack = new Stack<Node<Vec2>>();
    searchStack.Push(startNode);
    
    while (searchStack.TryPop(out var node))
    {
        visited.Add(node.Value, node);
        var validDirections = grid.Get(node.Value) switch
        {
            '^' => [directions[3]],
            'v' => [directions[2]],
            '<' => [directions[1]],
            '>' => [directions[0]],
            _ => directions
        };

        var validNeighbors = validDirections
            .Select(d => node.Value + d)
            .Where(p => grid.Contains(p) && CanMove(grid.Get(p), node.Value, p));
        
        foreach (var neighbor in validNeighbors)
        {
            if (visited.TryGetValue(neighbor, out var value))
            {
                if (IsSlope(grid.Get(node.Value)))
                {
                    // Already added the node from some other path, just need to add it to our children
                    node.AddEdge(value);
                }
            }
            else
            {
                var neighborNode = graph.AddNode(neighbor, node);
                searchStack.Push(neighborNode);
            }
        }
    }

    var longestPath = graph.GetLongestPath(start, goal);
    //DebugPrint(grid, longestPath);
    
    return longestPath;
}

long Part2(IReadOnlyList<string> lines)
{
    // We no longer have a DAG, so we can't just topologically sort.
    // There are only 4 nodes that connect to more than 1 path so we can make weighted edges
    return 0;
}

var lines = new List<string>(File.ReadAllLines("input.txt"));
var part1 = Part1(lines);
var part2 = Part2(lines);

Console.WriteLine($"Part 1: {part1}, Part 2: {part2}");

Runner.Benchmark(delegate
{
    Part1(lines);
    Part2(lines);
}, "Day 23");

internal class Node<T>(T value)
{
    public T Value { get; } = value;
    public Dictionary<Node<T>, int> Incoming { get; } = new();
    public Dictionary<Node<T>, int> Outgoing { get; } = new();

    public List<Node<T>> Children => Outgoing.Keys.ToList();

    public void AddEdge(Node<T> node, int weight = 1)
    {
        Outgoing[node] = weight;
        node.Incoming[this] = weight;
    }
}

internal class Graph<T>
    where T : class
{
    public List<Node<T>> Nodes { get; } = [];

    public Node<T> AddNode(T value, Node<T>? parentNode = null, int weight = 1)
    {
        var node = new Node<T>(value);
        parentNode?.AddEdge(node, weight);

        Nodes.Add(node);

        return node;
    }

    private List<Node<T>> TopologicalSort()
    {
        var stack = new Stack<Node<T>>();
        var visited = new HashSet<Node<T>>();
        
        TopologicalSortUtil(Nodes[0], visited, stack);
        
        var sortedList = new List<Node<T>>();
        while (stack.Count > 0)
        {
            sortedList.Add(stack.Pop());
        }
        
        return sortedList;
    }

    private static void TopologicalSortUtil(Node<T> node, HashSet<Node<T>> visited, Stack<Node<T>> stack)
    {
        if (visited.Contains(node))
        {
            return;
        }
        
        foreach (var child in node.Children)
        {
            TopologicalSortUtil(child, visited, stack);
        }

        visited.Add(node);
        stack.Push(node);
    }
    
    public long GetLongestPath(T start, T end)
    {
        var longestPath = new Dictionary<T, int>();

        var topologicalSort = TopologicalSort();
        foreach (var node in topologicalSort)
        {
            longestPath[node.Value] = 0;
        }

        longestPath[start] = 0;
        foreach (var node in topologicalSort)
        {
            foreach (var child in node.Children
                         .Where(child => longestPath[child.Value] <= longestPath[node.Value] + 1))
            {
                longestPath[child.Value] = longestPath[node.Value] + 1;
            }
        }

        if (longestPath[end] == 0)
        {
            return -1;
        }

        return longestPath.Values.Max();
    }
}