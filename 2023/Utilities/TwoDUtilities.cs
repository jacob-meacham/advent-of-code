namespace Utilities;

public static class TwoDUtilities
{
    public record Vec2(long X = 0, long Y = 0)
    {
        public static Vec2 operator +(Vec2 a, Vec2 b)
        {
            return new Vec2(a.X + b.X, a.Y + b.Y);
        }
    
        public static Vec2 operator -(Vec2 a, Vec2 b)
        {
            return new Vec2(a.X - b.X, a.Y - b.Y);
        }
    }

    public class Grid<TCell>(TCell[,] cells)
    {
        public TCell[,] Cells { get; } = cells;

        public int MaxRows => Cells.GetLength(0) - 1;
        public int MaxCols => Cells.GetLength(1) - 1;

        public static Grid<TCell> CreateFromInput(IReadOnlyList<string> lines, Func<char, TCell> convertFn)
        {
            var width = lines[0].Length;
            var height = lines.Count;
            var cells = new TCell[height, width];
            for (var x = 0; x < height; x++)
            {
                for (var y = 0; y < width; y++)
                {
                    cells[x, y] = convertFn(lines[x][y]);
                }
            }

            return new Grid<TCell>(cells);
        }

        public bool Contains(Vec2 pos)
        {
            return pos.X >= 0 && pos.X < Cells.GetLength(0) && pos.Y >= 0 && pos.Y < Cells.GetLength(1);
        }

        public ref TCell Get(Vec2 pos)
        {
            return ref Cells[pos.X, pos.Y];
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