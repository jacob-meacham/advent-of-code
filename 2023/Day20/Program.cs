using Utilities;

Dictionary<string, IModule> ParseModules(IEnumerable<string> lines)
{
    var modules = new Dictionary<string, IModule>();
    var conModules = new List<ConjunctionModule>();
    var destinations = new Dictionary<string, List<string>>();

    foreach (var l in lines)
    {
        var (name, outputs) = l.Split(" -> ");
        var destinationModules = outputs.Split(", ");
        switch (name[0])
        {
            case '&':
                name = name[1..];
                var conModule = new ConjunctionModule(name, destinationModules);
                conModules.Add(conModule);
                modules.Add(name, conModule);
                break;
            case '%':
                name = name[1..];
                modules.Add(name, new FlipFlopModule(name, destinationModules));
                break;
            default:
                modules.Add("broadcaster", new BroadcastModule(destinationModules));
                break;
        }

        foreach (var destinationModule in destinationModules)
        {
            if (!destinations.TryGetValue(destinationModule, out var list))
            {
                list = new List<string>();
                destinations[destinationModule] = list;
            }

            list.Add(name);
        }
    }

    foreach (var conModule in conModules)
    {
        conModule.AddInputModules(destinations[conModule.Name]);
    }

    return modules;
}

void PrintDebug(Pulse pulse)
{
    var highStr = pulse.High ? "high" : "low";
    Console.WriteLine($"{pulse.From} -{highStr}-> {pulse.To}");
}

long Part1(IEnumerable<string> lines)
{
    var modules = ParseModules(lines);
    var pulses = new Queue<Pulse>();
    var pulseCounts = new Dictionary<bool, long>
    {
        [true] = 0L,
        [false] = 0L
    };
    
    // TODO: Potentially memoize the state of all modules
    for (var _ = 0; _ < 1000; _++)
    {
        pulses.Enqueue(new Pulse("button","broadcaster", false));
        while (pulses.Count > 0)
        {
            var pulse = pulses.Dequeue();
            pulseCounts[pulse.High] += 1;
                    
            // PrintDebug(pulse);
            
            if (!modules.ContainsKey(pulse.To))
            {
                continue;
            }
            
            var newPulses = modules[pulse.To].ReceivePulse(pulse);
            pulses.EnqueueRange(newPulses);
        }
    }
    
    return pulseCounts[false] * pulseCounts[true];
}

long Part2(IEnumerable<string> lines)
{
    var modules = ParseModules(lines);
    // The insight here is that this is actually split up into a group of 4 binary counters, each of which have
    // some number of flip flops. We can look at the flip flop inputs as a binary number, and then just need
    // to calculate the lcm for when all 4 input binary counters are high
    var pulses = new Queue<Pulse>();
    var pressesNeededForHigh = new Dictionary<string, long>();

    var numPresses = 0L;
    while (true)
    {
        pulses.Enqueue(new Pulse("button","broadcaster", false));
        numPresses++;
        
        while (pulses.Count > 0)
        {
            var pulse = pulses.Dequeue();
            if (pulse is { To: "zg", High: true })
            {
                pressesNeededForHigh[pulse.From] = numPresses;
                break;
            }
                    
            // PrintDebug(pulse);
            
            if (!modules.ContainsKey(pulse.To))
            {
                continue;
            }
            
            var newPulses = modules[pulse.To].ReceivePulse(pulse);
            pulses.EnqueueRange(newPulses);
        }

        if (pressesNeededForHigh.Keys.Count == 4)
        {
            break;
        }
    }

    return MathUtilities.LCM(pressesNeededForHigh.Values);
}

var lines = new List<string>(File.ReadAllLines("input.txt"));
var part1 = Part1(lines);
var part2 = Part2(lines);

Console.WriteLine($"Part 1: {part1}, Part 2: {part2}");

Runner.Benchmark(delegate
{
    Part1(lines);
    Part2(lines);
}, "Day 20");

internal class FlipFlopModule(string name, IEnumerable<string> destinationModules) : IModule
{
    private bool _on = false;

    public IEnumerable<Pulse> ReceivePulse(Pulse pulse)
    {
        if (pulse.High)
        {
            return [];
        }

        _on = !_on;
        return destinationModules.Select(m => new Pulse(name, m, _on));
    }
}

internal class ConjunctionModule(string name, IEnumerable<string> destinationModules) : IModule
{
    public string Name { get; } = name;
    private readonly Dictionary<string, bool> _inputModules = new();

    public void AddInputModules(IEnumerable<string> moduleNames)
    {
        foreach (var moduleName in moduleNames)
        {
            _inputModules.TryAdd(moduleName, false);    
        }
    }

    public IEnumerable<Pulse> ReceivePulse(Pulse pulse)
    {
        _inputModules[pulse.From] = pulse.High;

        var isLow = _inputModules.All(pair => pair.Value);
        return destinationModules.Select(m => new Pulse(Name,m, !isLow));
    }
}

internal class BroadcastModule(IEnumerable<string> destinationModules) : IModule
{
    public IEnumerable<Pulse> ReceivePulse(Pulse pulse)
    {
        return destinationModules.Select(m => pulse with { From = "broadcaster", To = m });
    }
}

internal interface IModule
{
    IEnumerable<Pulse> ReceivePulse(Pulse pulse);
}

internal record Pulse(string From, string To, bool High);