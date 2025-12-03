namespace Part1;

internal class Program
{
    internal static void Main(string[] args)
    {
        string input = GetInput();
        IEnumerable<Range> ranges = ParseInput(input);
        long result = Solve(ranges);
        Console.WriteLine($"Result: {result}");
    }

    private static long Solve(IEnumerable<Range> ranges)
    {
        long sum = 0;

        foreach (Range range in ranges)
        {
            for (long i = range.Start; i <= range.End; i++)
            {
                if (NumberIsRepeated(i))
                {
                    sum += i;
                }
            }
        }

        return sum;
    }

    private static bool NumberIsRepeated(long number)
    {
        string numberString = number.ToString();

        if (numberString.Length % 2 == 1)
        {
            return false;
        }

        int halfLength = numberString.Length / 2;
        string firstHalf = numberString[..halfLength];
        string secondHalf = numberString[halfLength..];

        return firstHalf == secondHalf;
    }

    private static string GetInput()
    {
        return File.ReadAllText("../../../../../input.txt");
    }

    private static IEnumerable<Range> ParseInput(string input)
    {
        string[] lines = input.Trim().Split(",");
        List<Range> res = new List<Range>(lines.Length);

        foreach (string line in lines)
        {
            string[] parts = line.Split("-");
            long start = long.Parse(parts[0]);
            long end = long.Parse(parts[1]);
            Range range = new Range(start, end);
            res.Add(range);
        }

        return res;
    }
}

internal readonly struct Range(long start, long end)
{
    public long Start { get; } = start;
    public long End { get; } = end;
}
