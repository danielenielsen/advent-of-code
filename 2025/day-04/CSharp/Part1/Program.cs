namespace Part1;

internal static class Program
{
    private static void Main()
    {
        string input = GetInput();
        bool[,] paperGrid = ParseInput(input);
        int res = Solve(paperGrid);
        Console.WriteLine($"Result: {res}");
    }

    private static int Solve(bool[,] paperGrid)
    {
        int sum = 0;
        
        for (int row = 0; row < paperGrid.GetLength(0); row++)
        {
            for (int col = 0; col < paperGrid.GetLength(1); col++)
            {
                if (paperGrid[row, col] && paperGrid.CountSurroundingPaper(row, col) < 4)
                {
                    sum++;
                }
            }
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

internal static class GridExtensions
{
    public static int CountSurroundingPaper(this bool[,] grid, int i, int j)
    {
        (int, int)[] combinations = [
            (i + 1, j + 1),
            (i + 1, j - 1),
            (i - 1, j + 1),
            (i - 1, j - 1),
            (i, j + 1),
            (i + 1, j),
            (i, j - 1),
            (i - 1, j),
        ];

        int count = 0;
        
        foreach ((int i1, int j1) in combinations)
        {
            if (i1 < 0 || i1 >= grid.GetLength(0))
            {
                continue;
            }

            if (j1 < 0 || j1 >= grid.GetLength(1))
            {
                continue;
            }

            if (!grid[i1, j1])
            {
                continue;
            }

            count++;
        }

        return count;
    }
}
