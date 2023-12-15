using Utilities;

static long Hash(string s)
{
    return s.ToCharArray().Aggregate(0L, (l, c) => (l + c) * 17 % 256);
}

long Part1(IReadOnlyList<string> lines)
{
    var sum = lines[0].Split(",")
        .Select(Hash).Sum();
    return sum;
}

long Part2(IReadOnlyList<string> lines)
{
    var hashMap = new FixedSizeHashMap<string, long>(256, Hash);
    foreach (var line in lines[0].Split(","))
    {
        if (line.EndsWith('-'))
        {
            hashMap.Remove(line.Substring(0, line.Length-1));
        }
        else
        {
            var parts = line.Split("=");
            hashMap.Add(parts[0], long.Parse(parts[1]));
        }
    }

    return hashMap.GetAll().SelectMany(list =>
    {
        // Not the most pretty way to get the box number
        long boxNum = Hash(list.First().key);
        return list.Select((tuple, slotNum) => (boxNum + 1) * (slotNum + 1) * tuple.value);
    }).Sum();
}

var lines = new List<string>(File.ReadAllLines("input.txt"));
var part1 = Part1(lines);
var part2 = Part2(lines);

Console.WriteLine($"Part 1: {part1}, Part 2: {part2}");

Runner.Benchmark(delegate
{
    Part1(lines);
    Part2(lines);
}, "Day 15");

internal class FixedSizeHashMap<TKey, TValue>(int size, Func<TKey, long> hashFn)
{
    LinkedList<(TKey key, TValue value)>?[] Data { get; } = new LinkedList<(TKey key, TValue value)>[size];

    public IEnumerable<LinkedList<(TKey key, TValue value)>> GetAll()
    {
        foreach (var list in Data)
        {
            if (list is { First: not null })
            {
                yield return list;
            }
        }
    }
    
    public void Add(TKey key, TValue value)
    {
        var bucket = hashFn(key);
        Data[bucket] ??= [];

        var currentNode = FindByKey(Data[bucket]!, key);
        if (currentNode != null)
        {
            var newNode = new LinkedListNode<(TKey label, TValue focalLength)>((key, value));
            Data[bucket]?.AddAfter(currentNode, newNode);
            Data[bucket]?.Remove(currentNode);
        }
        else
        {
            Data[bucket]?.AddLast((key, value));
        }
    }
    
    public void Remove(TKey key)
    {
        var bucket = hashFn(key);
        if (Data[bucket] == null)
        {
            return;
        }
        
        var currentNode = FindByKey(Data[bucket]!, key);
        if (currentNode == null)
        {
            return;
        }
        
        Data[bucket]?.Remove(currentNode);
    }

    private static LinkedListNode<(TKey key, TValue value)>? FindByKey(LinkedList<(TKey key, TValue value)> list, TKey key)
    {
        var currentNode = list.First;
        while (currentNode != null)
        {
            if (currentNode.Value.key!.Equals(key))
            {
                return currentNode;
            }

            currentNode = currentNode.Next;
        }

        return null;
    }
}