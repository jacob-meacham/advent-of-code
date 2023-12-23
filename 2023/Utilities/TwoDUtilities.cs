namespace Utilities;

public static class TwoDUtilities
{
    public record Vec2(long X, long Y)
    {
        public long X { get; set; } = X;
        public long Y { get; set; } = Y;
        
        public static readonly Vec2 North = new(-1, 0);
        public static readonly Vec2 South = new(1, 0);
        public static readonly Vec2 East = new(0, 1);
        public static readonly Vec2 West = new(0, -1);
        
        public static Vec2 operator +(Vec2 a, Vec2 b)
        {
            return new Vec2(a.X + b.X, a.Y + b.Y);
        }
        
        public static Vec2 operator +(Vec2 a, (long x, long y) b)
        {
            return new Vec2(a.X + b.x, a.Y + b.y);
        }
        
        public static Vec2 operator *(Vec2 a, long b)
        {
            return new Vec2(a.X * b, a.Y * b);
        }
    
        public static Vec2 operator -(Vec2 a, Vec2 b)
        {
            return new Vec2(a.X - b.X, a.Y - b.Y);
        }
    }

    public class Grid<TCell>(TCell[,] cells)
    {
        public TCell?[,] Cells { get; } = cells;
        private int? _maxRows;
        private int? _maxCols;

        public int MaxRows
        {
            get { return _maxRows ??= Cells.GetLength(0) - 1; }
        }

        public int MaxCols
        {
            get { return _maxCols ??= Cells.GetLength(1) - 1; }
        }

        public static Grid<TCell> CreateFromInput(IReadOnlyList<string> lines, Func<char, int, int, TCell> convertFn)
        {
            var width = lines[0].Length;
            var height = lines.Count;
            var cells = new TCell[height, width];
            for (var x = 0; x < height; x++)
            {
                for (var y = 0; y < width; y++)
                {
                    cells[x, y] = convertFn(lines[x][y], x, y);
                }
            }

            return new Grid<TCell>(cells);
        }
        
        public IEnumerable<(TCell? cell, long x, long y)> GetCells()
        {
            for (var x = 0; x <= MaxRows; x++)
            {
                for (var y = 0; y <= MaxCols; y++)
                {
                    yield return (Cells[x, y], x, y);
                }
            }
        }

        public bool Contains(Vec2 pos)
        {
            return pos.X >= 0 && pos.X < Cells.GetLength(0) && pos.Y >= 0 && pos.Y < Cells.GetLength(1);
        }
        
        public bool Contains((long x, long y) pos)
        {
            return pos.x >= 0 && pos.x < Cells.GetLength(0) && pos.y >= 0 && pos.y < Cells.GetLength(1);
        }

        public ref TCell? Get(Vec2 pos)
        {
            return ref Cells[pos.X, pos.Y];
        }
        
        public ref TCell? Get((long x, long y) pos)
        {
            return ref Cells[pos.x, pos.y];
        }
    }
    
    public static long ManhattanDistance((long x, long y) pt1, (long x, long y) pt2)
    {
        return Math.Abs(pt1.x - pt2.x) + Math.Abs(pt1.y - pt2.y);
    }
    
    public static long ManhattanDistance(Vec2 pt1, Vec2 pt2)
    {
        return Math.Abs(pt1.X - pt2.X) + Math.Abs(pt1.Y - pt2.Y);
    }

    public abstract class AStarSolver<TNode> where TNode : notnull
    {
        protected abstract IEnumerable<TNode> GetNeighbors(TNode current);
        protected abstract long GetHeuristic(TNode node, TNode goal);
        protected abstract long GetCost(TNode current, TNode neighbor);
        protected abstract bool IsGoal(TNode node, TNode goal);

        public List<TNode>? Solve(TNode start, TNode goal)
        {
            return Solve(Enumerable.Repeat(start, 1), goal);
        }
    
        public List<TNode>? Solve(IEnumerable<TNode> starts, TNode goal)
        {
            var pathMap = new Dictionary<TNode, TNode?>();
            var gScore = new Dictionary<TNode, long>();
            var frontier = new PriorityQueue<TNode, long>();

            foreach (var s in starts)
            {
                frontier.Enqueue(s, 0);
                pathMap[s] = default;
                gScore[s] = 0;
            }

            while (frontier.Count > 0)
            {
                var current = frontier.Dequeue();
    
                if (IsGoal(current, goal))
                {
                    var path = new List<TNode>();
                    while(current != null)
                    {
                        path.Add(current);
                        pathMap.TryGetValue(current, out current);
                    }
                    path.Reverse();
                    return path;
                }
    
                foreach (var next in GetNeighbors(current))
                {
                    var newCost = gScore[current] + GetCost(current, next);
                    if (!gScore.ContainsKey(next) || newCost < gScore[next])
                    {
                        gScore[next] = newCost;
                        var priority = newCost + GetHeuristic(next, goal);
                        frontier.Enqueue(next, priority);
                        pathMap[next] = current;
                    }
                }
            }
    
            // If no path was found
            return null;
        }
    }
    
    // TODO: Parameterize
    public static void FloodFill(char[,] map, (long x, long y) startingPoint)
    {
        var height = map.GetLength(0);
        var width = map.GetLength(1);
    
        var stack = new Stack<(long x, long y)>();
        stack.Push(startingPoint);

        while (stack.Count > 0)
        {
            var currentPoint = stack.Pop();
        
            if (currentPoint.x < 0 || currentPoint.y < 0 || currentPoint.x >= height || currentPoint.y >= width)
            {
                continue;
            }
        
            if (map[currentPoint.x, currentPoint.y] != '.')
            {
                continue;
            }

            map[currentPoint.x, currentPoint.y] = 'O';
            stack.Push((currentPoint.x-1, currentPoint.y));
            stack.Push((currentPoint.x+1, currentPoint.y));
            stack.Push((currentPoint.x, currentPoint.y-1));
            stack.Push((currentPoint.x, currentPoint.y+1));
        }
    }
}