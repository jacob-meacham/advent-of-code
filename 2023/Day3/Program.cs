using System.Text;
using Utilities;

List<Cell> GetNeighbors(Cell[,] cells, int x, int y)
{
    return new List<Cell> 
    {
        cells[x+1, y],
        cells[x-1, y],
        cells[x, y+1],
        cells[x, y-1],
        cells[x+1, y-1],
        cells[x+1, y+1],
        cells[x-1, y-1],
        cells[x-1, y+1]
    };
}

(int Value, int Length) ParseValue(char[] line, int pos)
{
    var valueBuilder = new StringBuilder();
    while (pos < line.Length && Char.IsDigit(line[pos]))
    {
        valueBuilder.Append(line[pos++]);
    }
    return (int.Parse(valueBuilder.ToString()), valueBuilder.Length);
}

void SetValueCells(Cell[,] schematic, int x, int y, int value, int length)
{
    // Create the cells
    var cells = new List<Cell>();
    for (int i = 0; i < length; i++)
    {
        cells.Add(Cell.ValueCell(value));
    }
    
    // Link the cells
    for (int i = 0; i < length; i++)
    {
        cells[i].LinkedCells = cells;
        schematic[x, y + i] = cells[i];
    } 
}

void FillRow(Cell[,] schematic, int row, Cell value)
{
    for (int y = 0; y < schematic.GetLength(1); y++)
    {
        schematic[row,y] = value;
    }
}

void Parse(Cell[,] schematic, List<string> lines)
{
    // Fill our first and last row with empties to make bounds checks simpler
    FillRow(schematic, 0, Cell.EmptyCell());
    FillRow(schematic, lines.Count + 1, Cell.EmptyCell());
    for(int x = 0; x < lines.Count; x++)
    {
        // First and last column are empty to make bounds checks simpler
        schematic[x+1, 0] = Cell.EmptyCell();
        schematic[x+1, schematic.GetLength(1)-1] = Cell.EmptyCell();
        var line = lines[x].ToCharArray();
    
        int y = 0;
        while (y < line.Length)
        {
            if (line[y] == '.')
            {
                schematic[x+1, y+1] = Cell.EmptyCell();
                y += 1;
            } else if (char.IsDigit(line[y]))
            {
                // Get the extents of the values
                var result = ParseValue(line, y);
                SetValueCells(schematic, x+1, y+1, result.Value, result.Length);
                y += result.Length;
            }
            else
            {
                schematic[x+1, y+1] = Cell.SymbolCell(line[y].ToString());
                y += 1;
            }
        }
    }
}

// Could do a sparse data set instead, but engine schematic is pretty small
var allLines = new List<string>(File.ReadAllLines("input.txt"));

// Keeping the same memory for the schematic instead of reallocating on each pass saves about 0.5 ms
var schematic = new Cell[allLines[0].Length + 2, allLines.Count + 2];

int Part1(List<string> lines)
{
    Parse(schematic, lines);
    var values = new List<int>();
    for (var x = 0; x < schematic.GetLength(0); x++)
    {
        for (var y = 0; y < schematic.GetLength(1); y++)
        {
            if (schematic[x, y].Type != CellType.Symbol)
            {
                continue;
            }
            
            // Check for unvisited values in all directions
            var valueNeighbors = GetNeighbors(schematic, x, y).Where(cell => cell.Type == CellType.Value).ToList();
            foreach (var cell in valueNeighbors)
            {
                if (cell.Visited)
                {
                    continue;
                }
                
                values.Add(cell.Value);
                cell.MarkVisited();
            }
        }
    }

    return values.Sum();
}

int Part2(List<string> lines)
{
    Parse(schematic, lines);
    var values = new List<int>();
    for (var x = 0; x < schematic.GetLength(0); x++)
    {
        for (var y = 0; y < schematic.GetLength(1); y++)
        {
            // Only care about gears
            if (schematic[x, y].Type != CellType.Symbol || schematic[x, y].CellChar != "*")
            {
                continue;
            }
            
            // A bit hacky - we chose to use the raw grid instead of an undirected graph but that means we need to get only distinct values.
            var valueNeighbors = GetNeighbors(schematic, x, y).Where(cell => cell.Type == CellType.Value)
                .Where(cell =>
                {
                    if (cell.Visited)
                    {
                        return false;
                    }
                    
                    cell.MarkVisited();
                    return true;
                }).ToList();
            
            if (valueNeighbors.Count == 2)
            {
                values.Add(valueNeighbors.Aggregate(1, (acc, n) => n.Value * acc));
            }
        }
    }

    return values.Sum();
}

var part1 = Part1(allLines);
var part2 = Part2(allLines);

Console.WriteLine($"Part 1: {part1}, Part 2: {part2}");

Runner.Benchmark(delegate
{
    Part1(allLines);
    Part2(allLines);
}, "Day 3");

internal enum CellType
{
    Empty,
    Value,
    Symbol
}

internal class Cell
{
    public bool Visited { get; private set; }
    public CellType Type { get; }
    public int Value { get; }
    
    public string CellChar { get; }

    private List<Cell> _linkedCells;

    public List<Cell> LinkedCells
    {
        set => _linkedCells = value;
    }

    public static Cell EmptyCell()
    {
        return new Cell(CellType.Empty, new List<Cell>());
    }

    public static Cell SymbolCell(string cellChar)
    {
        return new Cell(CellType.Symbol, new List<Cell>(), 0, cellChar);
    }

    public static Cell ValueCell(int value)
    {
        return new Cell(CellType.Value, new List<Cell>(), value);
    }

    private Cell(CellType type, List<Cell> linkedCells, int value = 0, string cellChar = "")
    {
        Type = type;
        _linkedCells = linkedCells;
        Value = value;
        CellChar = cellChar;
        Visited = false;
    }
    
    public bool MarkVisited()
    {
        if (Visited)
        {
            return false;
        }
        
        Visited = true;
        foreach (var cell in _linkedCells)
        {
            cell.Visited = true;
        }
        return true;
    }
}