using System.Text;

namespace Part2;

internal static class Program
{
    private static void Main()
    {
        RunTests();
        string input = GetInput();
        Grid grid = new Grid(input);
        long res = grid.CalculateAllTimelines();
        Console.WriteLine($"Result: {res}");
    }

    private static string GetInput()
    {
        return File.ReadAllText("../../../../../input.txt");
    }

    private static void RunTests()
    {
        RunTest("Test 1", "......S......\n.............\n......^......\n.............", 2);
        RunTest("Test 2", "......S......\n.............\n......^......\n.............\n.....^.......\n.............", 3);
        RunTest("Test 3", "......S......\n.............\n......^......\n.............\n.....^.^.....\n.............", 4);
        RunTest("Test 4", "......S......\n.............\n......^......\n.............\n.....^.^.....\n.............\n......^......\n.............", 6);
        RunTest("Test input", ".......S.......\n...............\n.......^.......\n...............\n......^.^......\n...............\n.....^.^.^.....\n...............\n....^.^...^....\n...............\n...^.^...^.^...\n...............\n..^...^.....^..\n...............\n.^.^.^.^.^...^.\n...............", 40);
    }

    private static void RunTest(string testName, string input, long expected)
    {
        Grid grid = new Grid(input);
        long res = grid.CalculateAllTimelines();
        if (res != expected)
        {
            throw new Exception($"{testName}: Expected a result of {expected} but got {res}.");
        }
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
    private Dictionary<(int row, int col), long> _splitterTimelineLookup = new Dictionary<(int row, int col), long>();

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

    public long CalculateAllTimelines()
    {
        for (int i = _height - 1; i >= 0; i--)
        {
            for (int j = _width - 1; j >= 0; j--)
            {
                CellType currentCell = _cells[i, j];
                if (currentCell == CellType.Splitter)
                {
                    CalculateSingleSplitterTimelines(i, j);
                }
            }
        }

        (int row, int col) = _startPosition;
        CellType cell = _cells[row, col];
        while (cell != CellType.Splitter)
        {
            row++;
            cell = _cells[row, col];
        }

        return _splitterTimelineLookup[(row, col)];
    }

    private void CalculateSingleSplitterTimelines(int row, int col)
    {
        if (_cells[row, col] != CellType.Splitter)
        {
            throw new Exception($"Cell {row}, {col} is not a splitter.");
        }

        long timelines = 0;

        // Left beam
        int newRow = row;
        int newCol = col - 1;
        while (true)
        {
            CellType currentCellType = _cells[newRow, newCol];

            if (currentCellType == CellType.Splitter)
            {
                if (!_splitterTimelineLookup.TryGetValue((newRow, newCol), out long leftTimelines))
                {
                    throw new Exception("Expected lower splitters to already have their timelines calculated.");
                }
                
                timelines += leftTimelines;
                break;
            }

            newRow++;

            if (newRow >= _height)
            {
                timelines++;
                break;
            }
        }
        
        // Right beam
        newRow = row;
        newCol = col + 1;
        while (true)
        {
            CellType currentCellType = _cells[newRow, newCol];

            if (currentCellType == CellType.Splitter)
            {
                if (!_splitterTimelineLookup.TryGetValue((newRow, newCol), out long rightTimelines))
                {
                    throw new Exception("Expected lower splitters to already have their timelines calculated.");
                }
                
                timelines += rightTimelines;
                break;
            }

            newRow++;

            if (newRow >= _height)
            {
                timelines++;
                break;
            }
        }
        
        _splitterTimelineLookup[(row, col)] = timelines;
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
