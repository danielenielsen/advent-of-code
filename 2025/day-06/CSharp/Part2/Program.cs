namespace Part2;

internal static class Program
{
    private static void Main()
    {
        string input = GetInput();
        IEnumerable<NumberColumn> cols = ParseInput(input);
        long res = Solve(cols);
        Console.WriteLine($"Result: {res}");
    }

    private static long Solve(IEnumerable<NumberColumn> cols)
    {
        return cols.Select(x => x.Calculate()).Sum();
    }

    private static string GetInput()
    {
        return File.ReadAllText("../../../../../input.txt");
    }

    private static IEnumerable<NumberColumn> ParseInput(string input)
    {
        input = input.Replace("\r", "")[..^1];
        string[] lines = input.Split("\n");
        int height = lines.Length;
        int width = lines[0].Length;
        
        char[,] grid = new char[height, width];

        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                grid[i, j] = lines[i][j];
            }
        }
        
        int currentColumn = width - 1;
        int currentRow = 0;
        string currentNumber = "";
        
        List<long> numbers = new List<long>(height);
        List<NumberColumn> cols = new List<NumberColumn>(width);
        
        while (currentColumn >= 0)
        {
            char c = grid[currentRow, currentColumn];

            if (c is '+' or '*')
            {
                currentNumber = currentNumber.Trim();
                
                if (!long.TryParse(currentNumber, out long number))
                {
                    throw new Exception($"Could not parse '{currentNumber}' as a long.");
                }
                
                numbers.Add(number);
                
                Operation op = c switch
                {
                    '+' => Operation.Addition,
                    '*' => Operation.Multiplication,
                    _ => throw new InvalidOperationException(),
                };

                NumberColumn col = new NumberColumn(numbers.ToList(), op);
                cols.Add(col);
                
                currentRow = 0;
                currentColumn -= 2;
                currentNumber = "";
                numbers.Clear();
                continue;
            }
            
            currentNumber += c;
            currentRow++;

            if (currentRow >= height)
            {
                currentNumber = currentNumber.Trim();
                
                if (!long.TryParse(currentNumber, out long number))
                {
                    throw new Exception($"Could not parse '{currentNumber}' as a long.");
                }
                
                numbers.Add(number);
                currentNumber = "";
                
                currentRow = 0;
                currentColumn--;
            }
        }

        return cols;
    }
}

internal enum Operation
{
    Addition,
    Multiplication,
}

internal readonly struct NumberColumn(List<long> numbers, Operation operation)
{
    public long Calculate()
    {
        return operation switch
        {
            Operation.Addition => numbers.Sum(),
            Operation.Multiplication => numbers.Aggregate(1L, (a, b) => a * b),
            _ => throw new Exception($"Invalid operation: {operation}")
        };
    }
}
