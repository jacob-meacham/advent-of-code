using Utilities;

var random = new Random();

(int remainingEdges, int partA, int partB) Kargers(Graph graph, List<int> vertices)
{
    var componentCount = vertices.Select(idx => new KeyValuePair<int, int>(idx, 1)).ToDictionary();
    
    // Use https://en.wikipedia.org/wiki/Karger%27s_algorithm to determine a probabilistic min cut
    while (vertices.Count > 2)
    {
        // Pick a random edge
        var v1 = vertices[random.Next(vertices.Count)];
        var edges = graph.GetEdges(v1);
        var v2 = edges[random.Next(edges.Count)];
        
        graph.ContractEdge(v1, v2);
        
        componentCount[v1] += componentCount[v2];
        componentCount.Remove(v2);
        vertices.Remove(v2);
    }

    return (graph.GetEdges(vertices[0]).Count, componentCount[vertices[0]], componentCount[vertices[1]]);
}

long Part1(IReadOnlyList<string> lines)
{
    Dictionary<string, int> idToIdx = new();
    
    var curVertIdx = 0;

    void AddVert(string name)
    {
        if (idToIdx.TryAdd(name, curVertIdx))
        {
            curVertIdx++;
        }
    }
    
    foreach (var l in lines)
    {
        var (vertex, adjs) = l.Split(": ");
        var all = new[] { vertex }.Concat(adjs.Split(" "));
        foreach (var vert in all)
        {
            AddVert(vert);
        }
    }
    
    // Could probably do this in one pass but knowing the size is useful for the adjacency list
    var graph = new Graph(idToIdx.Count);
    foreach (var l in lines)
    {
        var (vertex, adjs) = l.Split(": ");
        graph.AddEdges(idToIdx[vertex], adjs.Split(" ").Select(a => idToIdx[a]));    
    }
    
    var vertices = idToIdx.Values.ToList();
    while (true)
    {
        var (remainingEdges, partA, partB) = Kargers(graph.Clone(), new List<int>(vertices));
        if (remainingEdges == 3)
        {
            return partA * partB;
        }
    }
}

var lines = new List<string>(File.ReadAllLines("input.txt"));
var part1 = Part1(lines);

Console.WriteLine($"Part 1: {part1}");

Runner.Benchmark(delegate
{
    Part1(lines);
}, "Day 25");

internal class Graph
{
    private readonly List<int>[] _adjacencyList;

    public Graph(int vertices)
    {
        _adjacencyList = new List<int>[vertices];
        for (var i = 0; i < vertices; i++)
        {
            _adjacencyList[i] = [];
        }
    }

    public void AddEdges(int vert, IEnumerable<int> neighbors)
    {
        foreach (var other in neighbors)
        {
            _adjacencyList[vert].Add(other);
            _adjacencyList[other].Add(vert);
        }
    }

    public List<int> GetEdges(int vert)
    {
        return _adjacencyList[vert];
    }
    
    public bool IsConnected(int vertex1, int vertex2)
    {
        return _adjacencyList[vertex1].Contains(vertex2) && _adjacencyList[vertex2].Contains(vertex1);
    }

    public void ContractEdge(int v1, int v2)
    {
        if (!IsConnected(v1, v2))
        {
            throw new ArgumentException($"Vertices {v1} and {v2} are not connected. Cannot contract edge.");
        }
        
        foreach (var vertex in _adjacencyList[v2])
        {
            _adjacencyList[vertex].Remove(v2);
         
            // Avoid self-loop
            if (vertex == v1)
            {
                continue;
            }
            _adjacencyList[v1].Add(vertex);
            _adjacencyList[vertex].Add(v1);
        }
        
        _adjacencyList[v2].Clear();
    }

    public Graph Clone()
    {
        var clone = new Graph(_adjacencyList.Length);
        for (var i = 0; i < _adjacencyList.Length; i++)
        {
            clone._adjacencyList[i] = [.._adjacencyList[i]];
        }

        return clone;
    }
}