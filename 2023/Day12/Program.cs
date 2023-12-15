using Utilities;

bool CouldBeBroken(char symbol)
{
    return symbol is '#' or '?';
}

bool CouldBeWorking(char symbol)
{
    return symbol is '.' or '?';
}

long Solve(string arrangement, IReadOnlyList<int> springs)
{
    // Uses an NFA for solving (thanks for Reddit for the inspo!)
    // NFA stores 4 pieces of state:
    // 1. current position in the arrangement, 2. current spring group,
    // 3. current number of springs placed, 4. whether we need a space to separate groups
    // state dictionaries store the number of arrangements found with that state
    // newStates is the next set of states we're moving to upon ingestion of the next input symbol

    var numPossible = 0L;
    
    var currentStates = new Dictionary<State, long>(32);
    var newStates = new Dictionary<State, long>(32);
    
    currentStates.Add(new State(0, 0, 0, false), 1);
    while (currentStates.Count > 0)
    {
        // Check and progress all state simultaneously by ingesting the next input symbol
        foreach (var state in currentStates.Keys)
        {
            var numStates = currentStates[state];
            
            if (state.ArrangePos == arrangement.Length)
            {
                if (state.SpringGroup == springs.Count)
                {
                    // Found possible positions in these states - we've placed all springs
                    numPossible += numStates;
                }

                // This is terminal for these states since we're at the end of the arrangement
                continue;
            }

            var inputSymbol = arrangement[state.ArrangePos];
            if (CouldBeBroken(inputSymbol) && state.SpringGroup < springs.Count && !state.NeedSpace)
            {
                // Next symbol could be a spring, and we have not started a spring group so this could be a working spring
                if (inputSymbol is '?' && state.CurSprings is 0)
                {
                    newStates.ApplyWithDefault(new State(
                        state.ArrangePos + 1,
                        state.SpringGroup,
                        state.CurSprings, 
                        false), (v) => v + numStates, 0);
                }

                if (state.CurSprings + 1 == springs[state.SpringGroup])
                {
                    // spring group can fit in these states
                    newStates.ApplyWithDefault(new State(
                        state.ArrangePos + 1,
                        state.SpringGroup + 1,
                        0,
                        true), (v) => v + numStates, 0);
                }
                else
                {
                    newStates.ApplyWithDefault(new State(
                        state.ArrangePos + 1,
                        state.SpringGroup,
                        state.CurSprings + 1,
                        false), (v) => v + numStates, 0);
                }
            }

            else if (CouldBeWorking(inputSymbol) && state.CurSprings == 0)
            {
                // Found a working spring
                newStates.ApplyWithDefault(new State(
                    state.ArrangePos + 1,
                    state.SpringGroup,
                    state.CurSprings,
                    false), (v) => v + numStates, 0);
            }
        }
        
        currentStates.Clear();
        currentStates = newStates.ToDictionary();
        newStates.Clear();
    }
    
    return numPossible;
}

long Part1(IEnumerable<string> lines)
{
    var input = new List<(string, int[])>();
    foreach (var split in lines.Select(l => l.Split(" ")))
    {
        input.Add((split[0], split[1].Split(",").Select(int.Parse).ToArray()));
    }
    
    return input.Select(t => Solve(t.Item1, t.Item2)).Sum();
}

long Part2(IEnumerable<string> lines)
{
    var input = new List<(string, int[])>();
    foreach (var split in lines.Select(l => l.Split(" ")))
    {
        var arrangement = string.Join("?", Enumerable.Repeat(split[0], 5));
        var springStr = string.Join(",", Enumerable.Repeat(split[1], 5));
        input.Add((arrangement, springStr.Split(",").Select(int.Parse).ToArray()));
    }
    
    return input.Select(t => Solve(t.Item1, t.Item2)).Sum();
}

var lines = new List<string>(File.ReadAllLines("input.txt"));
var part1 = Part1(lines);
var part2 = Part2(lines);

Console.WriteLine($"Part 1: {part1}, Part 2: {part2}");

Runner.Benchmark(delegate
{
    Part1(lines);
    Part2(lines);
}, "Day 12");

// I tried using a record here instead, but it was quite a bit slower
internal class State(int arrangePos, int springGroup, int curSprings, bool needSpace)
{
    public int ArrangePos { get; } = arrangePos;
    public int SpringGroup { get; } = springGroup;
    public int CurSprings { get; } = curSprings;
    public bool NeedSpace { get; } = needSpace;

    private bool Equals(State other)
    {
        return ArrangePos == other.ArrangePos && SpringGroup == other.SpringGroup && CurSprings == other.CurSprings && NeedSpace == other.NeedSpace;
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((State)obj);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(ArrangePos, SpringGroup, CurSprings, NeedSpace);
    }
}