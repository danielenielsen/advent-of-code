namespace Part2;

internal static class Program
{
    private static void Main()
    {
        string input = GetInput();
        PaperGrid paperGrid = new PaperGrid(input);
        int res = Solve(paperGrid);
        Console.WriteLine($"Result: {res}");
    }

    private static int Solve(PaperGrid paperGrid)
    {
        int sum = 0;

        while (true)
        {
            int removed = paperGrid.RemovePaper();

            if (removed == 0)
            {
                break;
            }
            
            sum += removed;
        }

        return sum;
    }

    private static string GetInput()
    {
        return File.ReadAllText("../../../../../input.txt");
    }

    private static bool[,] ParseInput(string input)
    {
        string[] lines = input.Trim().Replace("\r", "").Split("\n");
        
        int width = lines[0].Length;
        int height = lines.Length;

        if (width != height)
        {
            throw new Exception("Width and height must match.");
        }
        
        bool[,] paperGrid = new bool[width, height];

        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                char c = lines[i][j];

                bool hasPaper = c switch
                {
                    '@' => true,
                    '.' => false,
                    _ => throw new Exception($"Invalid character '{c}'."),
                };
                
                paperGrid[i, j] = hasPaper;
            }
        }

        return paperGrid;
    }
}

internal class PaperGrid
{
    public readonly int Width;
    public readonly int Height;
    public readonly bool[,] Grid;

    public PaperGrid(string input)
    {
        string[] lines = input.Trim().Replace("\r", "").Split("\n");
        
        int width = lines[0].Length;
        int height = lines.Length;

        if (width != height)
        {
            throw new Exception("Width and height must match.");
        }
        
        bool[,] paperGrid = new bool[width, height];

        for (int row = 0; row < height; row++)
        {
            for (int col = 0; col < width; col++)
            {
                char c = lines[row][col];

                bool hasPaper = c switch
                {
                    '@' => true,
                    '.' => false,
                    _ => throw new Exception($"Invalid character '{c}'."),
                };
                
                paperGrid[row, col] = hasPaper;
            }
        }

        Width = width;
        Height = height;
        Grid = paperGrid;
    }
    
    private IEnumerable<(int row, int col)> Iter() {
        for (int row = 0; row < Height; row++)
        {
            for (int col = 0; col < Width; col++)
            {
                yield return (row, col);
            }
        }
    }

    private int CountSurroundingPaper(int row, int col)
    {
        (int, int)[] combinations = [
            (row + 1, col + 1),
            (row + 1, col - 1),
            (row - 1, col + 1),
            (row - 1, col - 1),
            (row, col + 1),
            (row + 1, col),
            (row, col - 1),
            (row - 1, col),
        ];

        int count = 0;
        
        foreach ((int i1, int j1) in combinations)
        {
            if (i1 < 0 || i1 >= Height)
            {
                continue;
            }

            if (j1 < 0 || j1 >= Width)
            {
                continue;
            }

            if (!Grid[i1, j1])
            {
                continue;
            }

            count++;
        }

        return count;
    }

    private bool IsPaperAccessible(int row, int col)
    {
        if (!Grid[row, col])
        {
            return false;
        }

        if (CountSurroundingPaper(row, col) >= 4)
        {
            return false;
        }
        
        return true;
    }

    public int RemovePaper()
    {
        List<(int row, int col)> accessiblePaperPositions = new List<(int row, int col)>();
        
        foreach ((int row, int col) in Iter())
        {
            if (IsPaperAccessible(row, col))
            {
                accessiblePaperPositions.Add((row, col));
            }
        }

        foreach ((int row, int col) in accessiblePaperPositions)
        {
            Grid[row, col] = false;
        }

        return accessiblePaperPositions.Count;
    }
}
