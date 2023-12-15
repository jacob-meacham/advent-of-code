using Utilities;

long GetTotalValue(List<string> lines, bool jacksWild)
{
    var hands = lines.Select(l =>
    {
        var split = l.Split(" ");
        return new Hand(split[0], long.Parse(split[1]), jacksWild);
    }).Order();
    
    // the tuple is (rank, winnings)
    var result = hands
        .Aggregate((1L, 0L), (tuple, hand) => (tuple.Item1 + 1, tuple.Item2 + tuple.Item1 * hand.Bid));
    
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

class Hand : IComparable<Hand>
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

        if (most == 5)
        {
            return HandType.FiveOfAKind;
        }
        
        if (most == 4)
        {
            return HandType.FourOfAKind;
        }
        
        if (most == 3 && counts.Skip(1).Contains(2))
        {
            return HandType.FullHouse;
        }
        
        if (most == 3)
        {
            return HandType.ThreeOfAKind;
        }
        
        if (most == 2 && counts.Skip(1).Contains(2))
        {
            return HandType.TwoPair;
        }
        
        if (most == 2)
        {
            return HandType.Pair;
        }
        
        return HandType.HighCard;
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
        switch (card)
        {
            case 'A':
                return 14;
            case 'K':
                return 13;
            case 'Q':
                return 12;
            case 'J' when JacksWild:
            {
                return 1;
            }
            case 'J':
                return 11;
            case 'T':
                return 10;
            default:
                return long.Parse(card.ToString());
        }
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