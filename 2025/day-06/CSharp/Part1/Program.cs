namespace Part1;

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
        input = input.Replace("\r", "").Trim();

        List<string> lines = input.Split("\n").ToList();
        List<List<string>> grid = lines.Select(line => line.Split(" ").Where(x => !string.IsNullOrEmpty(x)).ToList()).ToList();

        int length = grid[0].Count;
        foreach (List<string> row in grid)
        {
            if (row.Count != length)
            {
                throw new Exception("Invalid length");
            }
        }

        List<List<string>> numbers = grid[..^1];
        List<string> operations = grid[^1];
        
        List<NumberColumn> cols = new List<NumberColumn>(length);

        for (int i = 0; i < length; i++)
        {
            List<long> parsedNumbers = [];

            for (int j = 0; j < numbers.Count; j++)
            {
                long number = long.Parse(numbers[j][i]);
                parsedNumbers.Add(number);
            }

            Operation op = operations[i] switch
            {
                "+" => Operation.Addition,
                "*" =>  Operation.Multiplication,
                _ => throw new Exception($"Invalid operation: {operations[i]}"),
            };

            cols.Add(new NumberColumn(parsedNumbers, op));
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
