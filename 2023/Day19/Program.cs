using Utilities;

Item ParseItem(string item)
{
    var strParts = item[1..^1].Split(",");
    var parts = strParts.Select(s => int.Parse(s.Split("=")[1])).ToList();

    return new Item(parts[0], parts[1], parts[2], parts[3]);
}

Dictionary<string, List<Rule>> ParseWorkflows(IEnumerable<string> lines)
{
    var workflows = new Dictionary<string, List<Rule>>();
    foreach (var l in lines)
    {
        var (workflowName, rulesStr) = l.Split('{');
        var rules = new List<Rule>();
        
        // Remove the trailing }
        foreach (var ruleStr in rulesStr[..^1].Split(','))
        {
            if (ruleStr.Contains('>'))
            {
                var (variable, rest) = ruleStr.Split('>');
                var (num, toWorkflow) = rest.Split(':');
                rules.Add(new GreaterThanRule(variable, int.Parse(num), toWorkflow));
            } else if (ruleStr.Contains('<'))
            {
                var (variable, rest) = ruleStr.Split('<');
                var (num, toWorkflow) = rest.Split(':');
                rules.Add(new LessThanRule(variable, int.Parse(num), toWorkflow));
            }
            else
            {
                rules.Add(new AlwaysRule(ruleStr));
            }
        }

        workflows[workflowName] = rules;
    }

    return workflows;
}

string ParseWorkflow(List<Rule> rules, Item item)
{
    foreach (var r in rules)
    {
        if (r.ApplyRule(item))
        {
            return r.ToWorkflow;
        }
    }

    return "R";
}

long Part1(IEnumerable<string> lines)
{
    var (instructionsStr, itemsStr) = string.Join("\n", lines).Split("\n\n");

    var instructions = ParseWorkflows(instructionsStr.Split("\n"));
    var items = itemsStr.Split("\n").Select(ParseItem);

    var accepted = new List<Item>();
    foreach (var item in items)
    {
        var workflow = "in";
        while (true)
        {
            workflow = ParseWorkflow(instructions[workflow], item);
            if (workflow == "A")
            {
                accepted.Add(item);
                break;
            } 
            
            if (workflow == "R")
            {
                break;
            }
        }
    }

    return accepted.Select(i => i.X + i.M + i.A + i.S).Sum();
}

long Part2(IReadOnlyList<string> lines)
{
    var sections = string.Join("\n", lines).Split("\n\n");

    var instructions = ParseWorkflows(sections[0].Split("\n"));
    
    // Solve backwards from A and then find the pathway that is the most constrained
    // Possibly able to do it with boolean logic
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
}, "Day 19");

internal record Item
{
    private readonly Dictionary<string, long> _data = new();
    
    public long X { get; }
    public long M { get; }
    public long A { get; }
    public long S { get; }
    
    public Item(long x, long m, long a, long s)
    {
        X = x;
        M = m;
        A = a;
        S = s;
        
        _data["x"] = X;
        _data["m"] = M;
        _data["a"] = A;
        _data["s"] = S;
    }

    public long GetByName(string s)
    {
        return _data[s];
    }
}

internal record AlwaysRule(string ToWorkflow) : Rule(ToWorkflow)
{
    public override bool ApplyRule(Item item)
    {
        return true;
    }
}

internal record GreaterThanRule(string Variable, long Num, string ToWorkflow) : Rule(ToWorkflow)
{
    public override bool ApplyRule(Item item)
    {
        return item.GetByName(Variable) > Num;
    }
}

internal record LessThanRule(string Variable, long Num, string ToWorkflow) : Rule(ToWorkflow)
{
    public override bool ApplyRule(Item item)
    {
        return item.GetByName(Variable) < Num;
    }
}

internal abstract record Rule(string ToWorkflow)
{
    public string ToWorkflow { get; } = ToWorkflow;
    public abstract bool ApplyRule(Item item);
}