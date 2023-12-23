using Utilities;

long GetTotalValue(List<string> lines, bool jacksWild)
{
    var hands = lines.Select(l =>
    {
        var split = l.Split(" ");
        return new Hand(split[0], long.Parse(split[1]), jacksWild);
    }).Order();
    
    var result = hands
        .Aggregate((rank: 1L, winnings: 0L), (tuple, hand) => (tuple.rank + 1, tuple.winnings + tuple.rank * hand.Bid));
    
    return result.Item2;
}

long Part1(List<string> lines)
{
    return GetTotalValue(lines, false);
}

long Part2(List<string> lines)
{
    return GetTotalValue(lines, true);
}

var lines = new List<string>(File.ReadAllLines("input.txt"));
var part1 = Part1(lines);
var part2 = Part2(lines);

Console.WriteLine($"Part 1: {part1}, Part 2: {part2}");

Runner.Benchmark(delegate
{
    Part1(lines);
    Part2(lines);
}, "Day 7");

internal enum HandType
{
    HighCard = 0,
    Pair,
    TwoPair,
    ThreeOfAKind,
    FullHouse,
    FourOfAKind,
    FiveOfAKind
}

internal class Hand : IComparable<Hand>
{
    private HandType Type { get; }
    private string Value { get; }
    public long Bid { get; }

    private bool JacksWild { get; }

    private HandType GetType(string value)
    {
        var cardCounts = new Dictionary<string, int>();
        foreach (var c in value)
        {
            if (cardCounts.ContainsKey(c.ToString()))
            {
                cardCounts[c.ToString()]++;
            }
            else
            {
                cardCounts[c.ToString()] = 1;
            }
        }

        return JacksWild ? GetTypeWild(cardCounts) : GetTypeNormal(cardCounts);
    }

    private static HandType GetTypeWild(Dictionary<string, int> cardCounts)
    {
        long numJacks = cardCounts.GetValueOrDefault("J");
        var counts = (from key in cardCounts.Keys where key != "J" select cardCounts[key]).ToList();

        counts = counts.OrderByDescending(n => n).ToList();
        var most = counts.ElementAtOrDefault(0) + numJacks;

        return most switch
        {
            5 => HandType.FiveOfAKind,
            4 => HandType.FourOfAKind,
            3 when counts.Skip(1).Contains(2) => HandType.FullHouse,
            3 => HandType.ThreeOfAKind,
            2 when counts.Skip(1).Contains(2) => HandType.TwoPair,
            2 => HandType.Pair,
            _ => HandType.HighCard
        };
    }

    private static HandType GetTypeNormal(Dictionary<string, int> cardCounts)
    {
        var countsSet = new HashSet<int>(cardCounts.Values);
        if (countsSet.Contains(5))
        {
            return HandType.FiveOfAKind;
        }
        
        if (countsSet.Contains(4))
        {
            return HandType.FourOfAKind;
        }
        
        if (countsSet.Contains(3) && countsSet.Contains(2))
        {
            return HandType.FullHouse;
        }
        
        if (countsSet.Contains(3))
        {
            return HandType.ThreeOfAKind;
        }
        
        if (countsSet.Contains(2) && cardCounts.Values.Count(v => v == 2) == 2)
        {
            return HandType.TwoPair;
        }
        
        if (countsSet.Contains(2))
        {
            return HandType.Pair;
        }
        
        return HandType.HighCard;
    }

    public Hand(string value, long bid, bool jacksWild)
    {
        Value = value;
        Bid = bid;
        JacksWild = jacksWild;
        Type = GetType(value);
    }
    
    private long GetCardValue(char card)
    {
        return card switch
        {
            'A' => 14,
            'K' => 13,
            'Q' => 12,
            'J' when JacksWild => 1,
            'J' => 11,
            'T' => 10,
            _ => long.Parse(card.ToString())
        };
    }
    
    public int CompareTo(Hand? other)
    {
        if (other == null)
        {
            return 1;
        }
        
        var typeCompare = Type.CompareTo(other.Type);
        if (typeCompare != 0)
        {
            return typeCompare;
        }

        return Value.Select((t, i) => GetCardValue(t).CompareTo(GetCardValue(other.Value[i])))
            .FirstOrDefault(cardCompare => cardCompare != 0);
    }
}