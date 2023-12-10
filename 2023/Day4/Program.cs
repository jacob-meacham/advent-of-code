using Utilities;

IEnumerable<int> GetNumbers(string part)
{
    return part.Trim()
        .Split(" ")
        .Where(s => s.Length > 0)
        .Select(s => int.Parse(s.Trim()));
}

int Part1(List<string> lines)
{
    var results = new List<int>();
    foreach (var line in lines)
    {
        var parts = line.Split(": ")[1].Split(" | ");
        var winners = new HashSet<int>(GetNumbers(parts[0]));
        var myNumbers = new HashSet<int>(GetNumbers(parts[1]));

        var count = myNumbers.Intersect(winners).Count();
        var result = count switch
        {
            0 => 0,
            1 => 1,
            _ => (int)Math.Pow(2, count-1)
        };
        results.Add(result);
    }

    return results.Sum();
}

int Part2(List<string> lines)
{
    var numWinners = new List<int>();
    foreach (var line in lines)
    {
        var parts = line.Split(": ")[1].Split(" | ");
        var winners = new HashSet<int>(GetNumbers(parts[0]));
        var myNumbers = new HashSet<int>(GetNumbers(parts[1]));

        numWinners.Add(myNumbers.Intersect(winners).Count());
    }
    
    // For each card, keep track of the additional cards per winner
    var numCards = Enumerable.Repeat(1, numWinners.Count).ToList();
    for (var i = 0; i < numCards.Count; i++)
    {
        for (var j = 0; j < numWinners[i]; j++)
        {
            numCards[i + j + 1] += numCards[i];
        }
    }

    return numCards.Sum();
}

var lines = new List<string>(File.ReadAllLines("input.txt"));

var part1 = Part1(lines);
var part2 = Part2(lines);

Console.WriteLine($"Part 1: {part1}, Part 2: {part2}");

Runner.Benchmark(delegate
{
    Part1(lines);
    Part2(lines);
}, "Day 4");