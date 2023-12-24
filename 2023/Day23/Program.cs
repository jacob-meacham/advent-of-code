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

// Another graph problem!
long Part1(IReadOnlyList<string> lines)
{
    var visitor = new Part1GraphVisitor();
    
    var start = new Vec2(0, lines[0].IndexOf('.'));
    var goal = new Vec2(lines.Count-1, lines[^1].IndexOf('.'));
    var graph = visitor.Create(lines, false);
    
    var longestPath = graph.GetLongestPath(start, goal);
    //DebugPrint(grid, longestPath);
    
    return longestPath;
}

long Part2(IReadOnlyList<string> lines)
{
    // We no longer have a DAG, so we can't just topologically sort.
    // There are only 4 nodes that connect to more than 1 path so we can make weighted edges
    var visitor = new Part2GraphVisitor();
    
    var goal = new Vec2(lines.Count-1, lines[^1].IndexOf('.'));
    var graph = visitor.Create(lines, true);
    
    graph.CollapseNodes();
    var maxDist = graph.Dfs(graph.Nodes[0], goal);

    return maxDist;
}

var lines = new List<string>(File.ReadAllLines("input.txt"));
var part1 = Part1(lines);
var part2 = Part2(lines);

Console.WriteLine($"Part 1: {part1}, Part 2: {part2}");

Runner.Benchmark(delegate
{
    Part1(lines);
    Part2(lines);
}, "Day 23", 2);

internal abstract class GraphVisitor
{
    protected abstract List<Vec2> ValidDirections(char c);
    protected abstract bool IsValidNeighbor(Grid grid, Vec2 curPos, Vec2 nextPos);

    protected abstract void VisitNeighbor(Grid grid, Dictionary<Vec2, Node<Vec2>> nodes, Node<Vec2> node, Vec2 neighbor,
        Stack<Node<Vec2>> searchStack, bool biDirectional);

    internal Graph<Vec2> Create(IReadOnlyList<string> lines, bool biDirectional)
    {
        var grid = Grid.CreateFromInput(lines, (c, _, _) => c);
        var start = new Vec2(0, lines[0].IndexOf('.'));
        var graph = new Graph<Vec2>(biDirectional);
    
        // Create our graph
        var nodes = new Dictionary<Vec2, Node<Vec2>>();
        var searchStack = new Stack<Node<Vec2>>();
        searchStack.Push(new Node<Vec2>(start));
    
        while (searchStack.TryPop(out var node))
        {
            nodes.TryAdd(node.Value, node);
            
            var c = grid.Get(node.Value);
            var validDirections = ValidDirections(c);

            var validNeighbors = validDirections
                .Select(d => node.Value + d)
                .Where(p => IsValidNeighbor(grid, node.Value, p));
        
            foreach (var neighbor in validNeighbors)
            {
                VisitNeighbor(grid, nodes, node, neighbor, searchStack, biDirectional);
            }
        }

        foreach (var node in nodes)
        {
            graph.AddNode(node.Value);
        }

        return graph;
    }
}

internal class Part1GraphVisitor : GraphVisitor
{
    protected override List<Vec2> ValidDirections(char c)
    {
        return c switch
        {
            '^' => [Statics.Directions[3]],
            'v' => [Statics.Directions[2]],
            '<' => [Statics.Directions[1]],
            '>' => [Statics.Directions[0]],
            _ => Statics.Directions
        };
    }

    protected override bool IsValidNeighbor(Grid grid, Vec2 curPos, Vec2 nextPos)
    {
        if (!grid.Contains(nextPos))
        {
            return false;
        }

        var c = grid.Get(nextPos);
        var direction = curPos - nextPos;
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

    protected override void VisitNeighbor(Grid grid, Dictionary<Vec2, Node<Vec2>> nodes, Node<Vec2> node, 
        Vec2 neighbor, Stack<Node<Vec2>> searchStack, bool biDirectional)
    {
        if (nodes.TryGetValue(neighbor, out var value))
        {
            var c = grid.Get(node.Value);
            if (c != '.' && c != '#')
            {
                // Already added the node from some other path non-directional path,
                // just need to add it to our children
                node.AddEdge(value, false);
            }
        }
        else
        {
            var neighborNode = new Node<Vec2>(neighbor);
            node.AddEdge(neighborNode, false);
            searchStack.Push(neighborNode);
        }
    }
}

internal class Part2GraphVisitor : GraphVisitor
{
    protected override List<Vec2> ValidDirections(char c)
    {
        return Statics.Directions;
    }

    protected override bool IsValidNeighbor(Grid grid, Vec2 curPos, Vec2 nextPos)
    {
        return grid.Contains(nextPos) && grid.Get(nextPos) != '#';
    }

    protected override void VisitNeighbor(Grid grid, Dictionary<Vec2, Node<Vec2>> nodes, Node<Vec2> node, Vec2 neighbor, Stack<Node<Vec2>> searchStack, bool biDirectional)
    {
        if (!nodes.TryGetValue(neighbor, out var neighborNode))
        {
            neighborNode = new Node<Vec2>(neighbor);
            nodes.Add(neighbor, neighborNode);
            
            searchStack.Push(neighborNode);
        }
        
        node.AddEdge(neighborNode, biDirectional);
    }
}

internal class Node<T>(T value)
{
    public T Value { get; set;  } = value;

    public List<(Node<T> node, int weight)> Neighbors { get; } = [];

    public void AddEdge(Node<T> neighbor, bool biDirectional)
    {
        if (Neighbors.All(tuple => tuple.node != neighbor))
        {
            Neighbors.Add((neighbor, 1));
        }

        if (!biDirectional)
        {
            return;
        }
        
        if (neighbor.Neighbors.All(tuple => tuple.node != this))
        {
            neighbor.Neighbors.Add((this, 1));
        }
    }
}

internal class Graph<T>(bool biDirectional)
    where T : class
{
    public List<Node<T>> Nodes { get; } = [];

    public Node<T> AddNode(T value, Node<T>? fromNode = null)
    {
        var node = new Node<T>(value);
        fromNode?.AddEdge(node, biDirectional);

        Nodes.Add(node);
        return node;
    }
    
    public void AddNode(Node<T> node)
    {
        Nodes.Add(node);
    }
    
    // Assumes the graph is fully connected
    public void CollapseNodes()
    {
        void ReplaceNeighbor(Node<T> nodeToReplace, Node<T> nodeToSearch, (Node<T> node, int weight) newNode)
        {
            for (var i = 0; i < nodeToSearch.Neighbors.Count; i++)
            {
                if (nodeToSearch.Neighbors[i].node != nodeToReplace)
                {
                    continue;
                }
                nodeToSearch.Neighbors[i] = (newNode.node, nodeToSearch.Neighbors[i].weight + newNode.weight);
                break;
            }
        }
        var removedNodes = new List<Node<T>>();
        var corridors = Nodes.Where(n => n.Neighbors.Count == 2);
        foreach (var n in corridors)
        {
            ReplaceNeighbor(n, n.Neighbors[0].node, n.Neighbors[1]);
            ReplaceNeighbor(n, n.Neighbors[1].node, n.Neighbors[0]);
            
            removedNodes.Add(n);
        }
       
        foreach (var node in removedNodes)
        {
            Nodes.Remove(node);
        }
    }

    public long Dfs(Node<T> start, T goal)
    {
        var visited = new HashSet<T>();
        return DfsHelper(visited, start, goal);
    }

    private static long DfsHelper(ISet<T> visited, Node<T> start, T goal)
    {
        if (start.Value.Equals(goal))
        {
            return 0L;
        }
        
        var maxDist = -1L;
        foreach (var (node, weight) in start.Neighbors)
        {
            if (!visited.Add(node.Value))
            {
                continue;
            }
            
            var dist = DfsHelper(visited, node, goal);
            if (dist != -1)
            {
                // Found a way to the goal
                maxDist = Math.Max(maxDist, dist + weight);    
            }
            visited.Remove(node.Value);
        }

        return maxDist;
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
        
        foreach (var (child, _) in node.Neighbors)
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
            foreach (var (child, _) in node.Neighbors
                         .Where(child => longestPath[child.node.Value] <= longestPath[node.Value] + 1))
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

internal static class Statics
{
    internal static readonly List<Vec2> Directions =
    [
        new Vec2(0, 1),
        new Vec2(0, -1),
        new Vec2(1, 0),
        new Vec2(-1, 0)
    ];
}