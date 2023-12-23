namespace Utilities;

public static class VectorUtilities
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
    
    public record Vec3(long X, long Y, long Z)
    {
        public long X { get; set; } = X;
        public long Y { get; set; } = Y;
        public long Z { get; set; } = Z;
        
        public static Vec3 operator +(Vec3 a, Vec3 b)
        {
            return new Vec3(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
        }
        
        public static Vec3 operator +(Vec3 a, (long x, long y, long z) b)
        {
            return new Vec3(a.X + b.x, a.Y + b.y, a.Z + b.z);
        }
    }

    public class Grid2D<TCell>(TCell[,] cells)
    {
        private TCell?[,] Cells { get; } = cells;
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

        public static Grid2D<TCell> CreateFromInput(IReadOnlyList<string> lines, Func<char, int, int, TCell> convertFn)
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

            return new Grid2D<TCell>(cells);
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
}