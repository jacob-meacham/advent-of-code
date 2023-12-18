using Utilities;
#pragma warning disable CS0162 // Unreachable code detected
#pragma warning disable CS8321 // Local function is declared but never used

const bool debugPath = false;

// Part 1, Example 1
// (long x, long y) INITIAL_DIRECTION = (0, -1);
// char STARTING_POINT_TYPE = 'F';

// Part 2, Example 1/2
// (long x, long y) INITIAL_DIRECTION = (0, -1);
// char STARTING_POINT_TYPE = 'F';

// Part 2, Example 3
//(long x, long y) INITIAL_DIRECTION = (0, 1);
//char STARTING_POINT_TYPE = '7';

// Full input
(long x, long y) initialDirection = (0, 1);
const char startingPointType = '7';

char GetDirectionArrow((long x, long y) direction)
{
    return direction switch
    {
        { x: 1 } => 'v',
        { x: -1 } => '^',
        { y: 1 } => '>',
        { y: -1 } => '<',
        _ => 'E'
    };
}

void PrintMap(char[,] map)
{
    for (var x = 0; x < map.GetLength(0); x++)
    {
        for (var y = 0; y < map.GetLength(1); y++)
        {
            Console.Write(map[x, y]);
        }
        Console.WriteLine();
    }
}

(long x, long y) ParseMap(char[,] map, IReadOnlyList<string> lines)
{
    (long x, long y) startingPoint = (0, 0);
    for (var x = 0; x < lines.Count; x++)
    {
        var line = lines[x].ToCharArray();
        for (var y = 0; y < line.Length; y++)
        {
            map[x, y] = line[y];
            if (map[x, y] != 'S')
            {
                continue;
            }
            map[x, y] = startingPointType;
            startingPoint = (x, y);
        }
    }

    return startingPoint;
}

List<(long x, long y)> GetPath(char[,] map, (long x, long y) startingPoint, (long x, long y) direction)
{
    var path = new List<(long x, long y)>();
    var currentPoint = startingPoint;
    
    while (true)
    {
        direction = map[currentPoint.x, currentPoint.y] switch
        {
            '|' => direction,
            '-' => direction,
            'L' => direction.x == 1 ? (0, 1) : (-1, 0),
            'J' => direction.x == 1 ? (0, -1) : (-1, 0),
            '7' => direction.x == -1 ? (0, -1) : (1, 0),
            'F' => direction.x == -1 ? (0, 1) : (1, 0),
            _ => throw new ArgumentOutOfRangeException(nameof(direction))
        };
        
        // if (debugPath)
        // {
        //     map[currentPoint.x, currentPoint.y] = GetDirectionArrow(direction);
        // }

        currentPoint = (currentPoint.x + direction.x, currentPoint.y + direction.y);
        path.Add(currentPoint);
        
        if (currentPoint.x == startingPoint.x && currentPoint.y == startingPoint.y)
        {
            break;
        }
    }

    // if (debugPath)
    // {
    //     PrintMap(map);
    // }

    return path;
}

long Part1(char[,] map, IReadOnlyList<string> lines)
{
    var startingPoint= ParseMap(map, lines);
    var path = GetPath(map, startingPoint, initialDirection);
    
    return path.Count / 2;
}

long Part2(char[,] map, IReadOnlyList<string> lines)
{
    var startingPoint= ParseMap(map, lines);
    var pathSet = new HashSet<(long x, long y)>(GetPath(map, startingPoint, initialDirection));
    
    // clean up map:
    for (var x = 0; x < map.GetLength(0); x++)
    {
        for (var y = 0; y < map.GetLength(1); y++)
        {
            if (pathSet.Contains((x, y)))
            {
                continue;
            }

            // Turn all non-path to ground
            map[x, y] = '.';
        }
    }
    
    var area = 0;
    for (var x = 0; x < map.GetLength(0); x++)
    {
        var lineParity = 0;
        var parityTypes = new HashSet<char> { '|', 'J', 'L' };
        for (var y = 0; y < map.GetLength(1); y++)
        {
            // Anytime we cross a wall where we enter at the bottom, update cardinality
            if (parityTypes.Contains(map[x, y]))
            {
                lineParity += 1;
            }

            if (map[x, y] != '.')
            {
                continue;
            }
            
            if (lineParity % 2 != 0)
            {
                area++;
                map[x, y] = 'I';
            }
            else
            {
                map[x, y] = 'O';
            }
        }
    }
    
    if (debugPath)
    {
        PrintMap(map);
    }

    return area;
}

var lines = new List<string>(File.ReadAllLines("input.txt"));
var map = new char[lines.Count, lines[0].Length];

var part1 = Part1(map, lines);
var part2 = Part2(map, lines);

Console.WriteLine($"Part 1: {part1}, Part 2: {part2}");

Runner.Benchmark(delegate
{
    Part1(map, lines);
    Part2(map, lines);
}, "Day 10");
