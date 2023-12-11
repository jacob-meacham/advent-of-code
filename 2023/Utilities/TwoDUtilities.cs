namespace Utilities;

public static class TwoDUtilities
{
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