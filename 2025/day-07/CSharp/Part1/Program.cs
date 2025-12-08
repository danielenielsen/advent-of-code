using System.Text;

namespace Part1;

internal static class Program
{
    private static void Main()
    {
        string input = GetInput();
        Grid grid = new Grid(input);
        long res = grid.PropagateTachyonBeams();
        Console.WriteLine($"Result: {res}");
    }

    private static string GetInput()
    {
        return File.ReadAllText("../../../../../input.txt");
    }
}

internal enum CellType
{
    Empty,
    Start,
    Splitter,
    TachyonBeam,
}

internal static class CellExtensions
{
    public static char GetChar(this CellType cellType)
    {
        return cellType switch
        {
            CellType.Empty => '.',
            CellType.Start => 'S',
            CellType.Splitter => '^',
            CellType.TachyonBeam => '|',
            _ => throw new ArgumentOutOfRangeException(nameof(cellType), cellType, null),
        };
    }
}

internal class Grid
{
    private CellType[,] _cells;
    private int _width => _cells.GetLength(1);
    private int _height => _cells.GetLength(0);

    private (int row, int column) _startPosition;

    public Grid(string input)
    {
        input = input.Replace("\r", "").Trim();
        
        string[] lines = input.Split("\n");
        bool linesSameLength = lines.SkipLast(1).Zip(lines.Skip(1)).All(tuple => tuple.First.Length == tuple.Second.Length);
        if (!linesSameLength)
        {
            throw new Exception("Not all rows are the same length in the input.");
        }
        
        int height = lines.Length;
        int width = lines[0].Length;
        _cells = new CellType[height, width];

        (int row, int col)? startPosition = null;
        
        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                char c = lines[i][j];
                CellType type = c switch
                {
                    'S' => CellType.Start,
                    '^' => CellType.Splitter,
                    '.' => CellType.Empty,
                    _ => throw new Exception($"Unknown cell type: {c}"),
                };

                if (type == CellType.Start)
                {
                    if (startPosition != null)
                    {
                        throw new Exception($"Start position is already set to {startPosition}, tried to set it to {(i, j)}.");
                    }
                    
                    startPosition = (i, j);
                }
                
                _cells[i, j] = type;
            }
        }

        if (startPosition == null)
        {
            throw new Exception("No start position found");
        }
        
        _startPosition = startPosition.Value;
    }

    public long PropagateTachyonBeams()
    {
        long splitCount = 0;
        
        Queue<(int row, int col)> queue = new Queue<(int row, int col)>();
        queue.Enqueue(_startPosition);

        while (queue.Count > 0)
        {
            (int row, int col) = queue.Dequeue();
            
            while (true)
            {
                if (row < 0 || row >= _cells.GetLength(0))
                {
                    throw new Exception($"Row out of range: {row}");
                }
                
                if (col < 0 || col >= _cells.GetLength(1))
                {
                    throw new Exception($"Col out of range: {col}");
                }
                
                CellType currentCellType = _cells[row, col];

                if (currentCellType == CellType.TachyonBeam)
                {
                    break;
                }

                if (currentCellType == CellType.Splitter)
                {
                    splitCount++;
                    queue.Enqueue((row, col - 1));
                    queue.Enqueue((row, col + 1));

                    break;
                }

                _cells[row, col] = CellType.TachyonBeam;
                row++;

                if (row >= _height)
                {
                    break;
                }
            }
        }

        return splitCount;
    }

    public override string ToString()
    {
        StringBuilder sb = new StringBuilder(_height * _width + _height);

        for (int i = 0; i < _height; i++)
        {
            for (int j = 0; j < _width; j++)
            {
                sb.Append(_cells[i, j].GetChar());
            }
            sb.Append('\n');
        }

        return sb.ToString();
    }
}
