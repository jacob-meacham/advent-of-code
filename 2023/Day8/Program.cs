using System.Text.RegularExpressions;
using Utilities;

long GetPathLengthToNode(Node? node, string value, IEnumerable<char> rules)
{
    long steps = 0;
    foreach (var d in rules)
    {
        steps++;
        node = d switch
        {
            'L' => node?.Left,
            'R' => node?.Right,
            _ => throw new ArgumentOutOfRangeException(nameof(node))
        };

        if (node?.Value.EndsWith(value) ?? false)
        {
            break;
        }
    }

    return steps;
}
    
long Part1(IReadOnlyList<string> lines)
{
    var graph = new Graph();
    foreach (var l in lines.Skip(2))
    {
        ParseNodeLine(graph, l);
    }

    var rules = lines[0].ToCharArray().Cycle();
    return GetPathLengthToNode(graph.GetNode("AAA"), "ZZZ", rules);
}

long Part2(IReadOnlyList<string> lines)
{
    var graph = new Graph();
    var startingNodes = lines.Skip(2)
        .Select(l => ParseNodeLine(graph, l))
        .Where(node => node.Value.EndsWith('A'))
        .ToList();

    var pathLengths = startingNodes.Select(n =>
    {
        var rules = lines[0].ToCharArray().Cycle();
        // Assumes that each node only gets to 1 terminal node
        return GetPathLengthToNode(n, "Z", rules);
    });

    return MathUtilities.LCM(pathLengths);
}

var lines = new List<string>(File.ReadAllLines("input.txt"));
var part1 = Part1(lines);
var part2 = Part2(lines);

Console.WriteLine($"Part 1: {part1}, Part 2: {part2}");

Runner.Benchmark(delegate
{
    Part1(lines);
    Part2(lines);
}, "Day 8");
return;

Node ParseNodeLine(Graph graph, string l)
{
    var matches = LineRegex().Matches(l)[0];
    var nodeName = matches.Groups[1].Value;
    var leftName = matches.Groups[2].Value;
    var rightName = matches.Groups[3].Value;
        
    var node = graph.AddNode(nodeName);
    var leftNode = graph.AddNode(leftName);
    var rightNode = graph.AddNode(rightName);
    node.Left = leftNode;
    node.Right = rightNode;

    return node;
}

internal class Graph
{
    private readonly Dictionary<string, Node> _graph = new(); 
    public Node AddNode(string value)
    {
        if (_graph.TryGetValue(value, out var n))
        {
            return n;
        }

        Node newNode = new Node(value);
        _graph.Add(value, newNode);
        return newNode;
    }

    public Node GetNode(string value)
    {
        return _graph[value];
    }
}

internal class Node(string value)
{
    public Node? Left { get; set; }
    public Node? Right { get; set; }

    public string Value { get; } = value;
}

internal partial class Program
{
    [GeneratedRegex(@"([A-Z]{3}) = \(([A-Z]{3}), ([A-Z]{3})\)")]
    private static partial Regex LineRegex();
}