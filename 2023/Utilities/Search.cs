namespace Utilities;

public class Search
{
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
}