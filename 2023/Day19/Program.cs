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

    var workflows = ParseWorkflows(instructionsStr.Split("\n"));
    var items = itemsStr.Split("\n").Select(ParseItem);

    var accepted = new List<Item>();
    foreach (var item in items)
    {
        var workflowName = "in";
        while (true)
        {
            workflowName = ParseWorkflow(workflows[workflowName], item);
            if (workflowName == "A")
            {
                accepted.Add(item);
                break;
            } 
            
            if (workflowName == "R")
            {
                break;
            }
        }
    }

    return accepted.Select(i => i.X + i.M + i.A + i.S).Sum();
}

long Part2(IEnumerable<string> lines)
{
    var (instructions, _) = string.Join("\n", lines).Split("\n\n");
    var workflows = ParseWorkflows(instructions.Split("\n"));

    var acceptedRanges = new List<ItemRange>();
    var workflowSteps = new Stack<(string workflow, ItemRange itemRange)>();
    workflowSteps.Push(("in",
        new ItemRange(
            new Range(1, 4000),
            new Range(1, 4000),
            new Range(1, 4000),
            new Range(1, 4000))));

    void WorkflowHelper(string workflowName, ItemRange itemRange)
    {
        switch (workflowName)
        {
            case "A":
            {
                acceptedRanges?.Add(itemRange);
                break;
            }
            case "R":
            {
                // Reject the accepted parts
                break;
            }
            default:
            {
                workflowSteps?.Push((workflowName, itemRange));
                break;
            }
        }
    }
    
    while (workflowSteps.Count > 0)
    {
        var (workflow, itemRange) = workflowSteps.Pop();
        
        // Use each rule to split the range and then follow the new workflow
        var rules = workflows[workflow];
        foreach (var rule in rules[..^1])
        {
            var (acceptedRange, rejectedRange) = rule.Split(itemRange!);
            itemRange = rejectedRange;

            WorkflowHelper(rule.ToWorkflow, acceptedRange!);
        }
        
        // Parse the remainder through the final rule
        WorkflowHelper(rules.Last().ToWorkflow, itemRange!);
    }
    
    return acceptedRanges
        .Select(i => i.X.Extents * i.M.Extents * i.A.Extents * i.S.Extents)
        .Sum();
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

internal record Range(long Min, long Max)
{
    public long Extents => Max - Min + 1;
}

internal record ItemRange
{
    private readonly Dictionary<string, Range> _data = new();

    public Range X => _data["x"];
    public Range M => _data["m"];
    public Range A => _data["a"];
    public Range S => _data["s"];
    
    public ItemRange(Range x, Range m, Range a, Range s)
    {
        _data["x"] = x;
        _data["m"] = m;
        _data["a"] = a; 
        _data["s"] = s;
    }

    public ItemRange(ItemRange other)
    {
        _data = new Dictionary<string, Range>
        {
            ["x"] = other.X,
            ["m"] = other.M,
            ["a"] = other.A,
            ["s"] = other.S
        };
    }

    public Range GetByName(string s)
    {
        return _data[s];
    }

    public void SetByName(string s, Range r)
    {
        _data[s] = r;
    }
}

internal record Item
{
    private readonly Dictionary<string, long> _data = new();
    
    public long X => _data["x"];
    public long M => _data["m"];
    public long A => _data["a"];
    public long S => _data["s"];
    
    public Item(long x, long m, long a, long s)
    {
        _data["x"] = x;
        _data["m"] = m;
        _data["a"] = a;
        _data["s"] = s;
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
    
    public override (ItemRange? accepted, ItemRange? rejected) Split(ItemRange itemRange)
    {
        return (new ItemRange(itemRange), null);
    }
}

internal record GreaterThanRule(string Variable, long Num, string ToWorkflow) : Rule(ToWorkflow)
{
    public override bool ApplyRule(Item item)
    {
        return item.GetByName(Variable) > Num;
    }
    
    public override (ItemRange? accepted, ItemRange? rejected) Split(ItemRange itemRange)
    {
        var accepted = new ItemRange(itemRange);
        var rejected = new ItemRange(itemRange);
        
        accepted.SetByName(Variable, itemRange.GetByName(Variable) with { Min = Num + 1 });
        rejected.SetByName(Variable, itemRange.GetByName(Variable) with { Max = Num });

        return (accepted, rejected);
    }
}

internal record LessThanRule(string Variable, long Num, string ToWorkflow) : Rule(ToWorkflow)
{
    public override bool ApplyRule(Item item)
    {
        return item.GetByName(Variable) < Num;
    }

    public override (ItemRange? accepted, ItemRange? rejected) Split(ItemRange itemRange)
    {
        var accepted = new ItemRange(itemRange);
        var rejected = new ItemRange(itemRange);
        
        accepted.SetByName(Variable, itemRange.GetByName(Variable) with { Max = Num - 1 });
        rejected.SetByName(Variable, itemRange.GetByName(Variable) with { Min = Num });

        return (accepted, rejected);
    }
}

internal abstract record Rule(string ToWorkflow)
{
    public string ToWorkflow { get; } = ToWorkflow;
    public abstract bool ApplyRule(Item item);
    
    public abstract (ItemRange? accepted, ItemRange? rejected) Split(ItemRange itemRange);
}